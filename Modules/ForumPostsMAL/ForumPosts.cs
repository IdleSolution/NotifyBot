using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using NotifyBot.Interfaces;

namespace NotifyBot.Modules.ForumPostsMAL
{
    public class ForumPosts : IIntervalActions
    {
        public string Url { get; set; }
        public SocketTextChannel Channel { get; set; }
        private const string PostsStoragePath = @"C:\Users\idles\Desktop\NotifyBot\postsStorage.txt";
        private readonly List<FrequentPosts> _posts = new List<FrequentPosts>();

        public ForumPosts(SocketTextChannel channel, string url)
        {
            Url = url;
            Channel = channel;
        }

        public void StartExecution()
        {
            GetNewestPostsAsync();
        }
        
        private async void GetNewestPostsAsync()
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(Url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            
            var postsHtml = htmlDocument.DocumentNode
                .Descendants("tr").Where(node => node.GetAttributeValue("id", "").Contains("topicRow")).Take(5);

            foreach (var post in postsHtml)
            {
                var title = post.Descendants("td")
                    .First(node => node.GetAttributeValue("class", "").Equals("forum_boardrow1"))
                    .Descendants("a").First().InnerHtml;
                
                var href = post.Descendants("td")
                    .First(node => node.GetAttributeValue("class", "").Equals("forum_boardrow1")).Descendants("a")
                    .First().Attributes["href"].Value;
                
                _posts.Add(new FrequentPosts()
                {
                    Title = title,
                    Href = href
                });
            }
            
            CheckForChanges();
        }

        private void CheckForChanges()
        {
            var text = "";
            var previousPosts = File.ReadAllText(PostsStoragePath).Split(" ");
            var changed = false;

            foreach (var post in _posts)
            {
                if (!previousPosts.Contains(post.Title.Replace(" ", "")))
                {
                    GetSinglePostAsync(post.Href);
                    changed = true;
                }
                text += post.Title.Replace(" ", "") + " ";
            }

            if (changed)
            {
                File.WriteAllText(PostsStoragePath, text);
                changed = false;
            }
        }

        private async void GetSinglePostAsync(string href)
        {
            var id = href.Substring(href.Length - 8);
            var httpClient = new HttpClient();
            var htmlDocument = new HtmlDocument();
            var increment = 0;

            while (true)
            {
                var link = href.Substring(0, href.Length - 30) + $"&show={increment}";
                var html = await httpClient.GetStringAsync(link);

                htmlDocument.LoadHtml(html);

                var checker = htmlDocument.DocumentNode
                    .Descendants("div").First(node => node.GetAttributeValue("id", "").Equals("contentWrapper")).InnerHtml;
                
                if (checker.Contains($"forumMsg{id}"))
                {
                    var post = htmlDocument.DocumentNode
                        .Descendants("div").First(node => node.GetAttributeValue("id", "").Equals($"message{id}"));

                    SendNotification(post.InnerHtml, href);
                    break;
                }

                increment += 50;
            }
        }

        private async void SendNotification(string desc, string href)
        {
            var message = new EmbedBuilder {Title = $"New MAL post from {GetUsername()}!", Description = desc}.Build();

            await Channel.SendMessageAsync("", false, message);
        }

        // I don't like regex, okay?
        private string GetUsername()
        {
            var username = new StringBuilder();
            var remember = false;
            
            for (int i = 0; i < Url.Length - 1; i++)
            {
                if (remember)
                {
                    if (Url[i] == '&' && Url[i + 1] == 'q')
                    {
                        break;
                    }
                    username.Append(Url[i]);
                }
                if (Url[i] == 'u' && Url[i + 1] == '=')
                {
                    remember = true;
                    i++;
                }
            }

            return username.ToString();
        }
    }
}