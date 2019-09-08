using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using NotifyBot.Interfaces;

namespace NotifyBot.Modules.MangadexChapter
{
    public class Chapters : IIntervalActions
    {
        public SocketTextChannel Channel { get; set; }
        private const string ChaptersStoragePath = @"C:\Users\idles\Desktop\NotifyBot\chaptersStorage.txt";
        private FrequentChapter _chapter;
        public string Url { get; set; }

        public Chapters(SocketTextChannel channel, string url)
        {
            Channel = channel;
            Url = url;
        }

        public void StartExecution()
        {
            GetNewestChaptersAsync();
        }

        private async void GetNewestChaptersAsync()
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(Url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var chapterHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("row no-gutters")).ToList();

            var chapterNumber = chapterHtml[1]
                .Descendants("div").First(node => node.GetAttributeValue("class", "").Contains("chapter-row"))
                .Attributes["data-chapter"].Value;

            var chapterLang = chapterHtml[1]
                .Descendants("span").First(node => node.GetAttributeValue("class", "").Contains("flag"))
                .Attributes["title"].Value;

            var chapterHref = chapterHtml[1]
                .Descendants("a").First(node => node.GetAttributeValue("class", "").Equals("text-truncate"))
                .Attributes["href"].Value;

            var chapterName = htmlDocument.DocumentNode
                .Descendants("span").First(node => node.GetAttributeValue("class", "").Equals("mx-1")).InnerHtml;

            _chapter = new FrequentChapter() {Href = chapterHref, Number = chapterNumber, Lang = chapterLang, Name = chapterName};
            
            CheckForChanges();
        }

        private void CheckForChanges()
        {
            var previousNumber = File.ReadAllText(ChaptersStoragePath);

            if (_chapter.Number != previousNumber && _chapter.Lang == "English")
            {
                SendNotification();
                File.WriteAllText(ChaptersStoragePath, _chapter.Number);
            }
        }

        private async void SendNotification()
        {
            var embed = new EmbedBuilder();
            embed.Title = "New chapter available!";
            embed.Description = $"{_chapter.Number}th chapter of {_chapter.Name} just came out on mangadex.org!";
            var message = embed.WithUrl("https://mangadex.org" + _chapter.Href).Build();
            
            await Channel.SendMessageAsync("", false, message);
        }
    }
}