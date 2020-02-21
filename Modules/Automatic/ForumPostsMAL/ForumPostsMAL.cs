using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using NotifyBot.Interfaces;
using NotifyBot.Services;

namespace NotifyBot.Modules.ForumPostsMAL
{
    public class ForumPostsMal : IIntervalActions
    {
        public string Url { get; set; }
        public SocketTextChannel Channel { get; set; }
        private readonly List<FrequentPosts> _posts = new List<FrequentPosts>();
        
        public ForumPostsMal(string url)
        {
            Url = url;
        }

        public ForumPostsMal(SocketTextChannel channel, string url)
        {
            Url = url;
            Channel = channel;
        }

        public async void StartExecution()
        {
            await GetNewestPostsAsync();
            CheckForChanges();
        }
        
        public async Task<List<FrequentPosts>> GetNewestPostsAsync()
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

            return _posts;
        }

        private void CheckForChanges()
        {
            var previousPosts = File.ReadAllLines(Paths.PostsStorageMal);
            var userNumber = 0;
            var certainPreviousPosts = new List<string>();
            for (int i = 0; i < previousPosts.Length; i++)
            {
                if (previousPosts[i].Split(" ")[0] == Url)
                {
                    userNumber = i;
                    var tempPosts = previousPosts[i].Split(" ");
                    for (int j = 1; j < tempPosts.Length; j++)
                    {
                        certainPreviousPosts.Add(tempPosts[j]);
                    }
                }
            }
 
            var text = "";
            var changed = false;

            foreach (var post in _posts)
            {
                if (!certainPreviousPosts.Contains(post.Title.Replace(" ", "")))
                {
                    GetSinglePostAsync(post.Href);
                    changed = true;
                }
                text += post.Title.Replace(" ", "") + " ";
            }

            if (changed)
            {
                previousPosts[userNumber] = Url + " " + text;
                File.WriteAllLines(Paths.ChaptersStorageMangadex, previousPosts);
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
            var message = new EmbedBuilder {Title = $"New MAL post from {GetMalUsernameFromLink()}!", Description = desc}.Build();

            await Channel.SendMessageAsync("", false, message);
        }

        // I don't like regex, okay?
        public string GetMalUsernameFromLink()
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