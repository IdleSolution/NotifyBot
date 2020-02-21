using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using NotifyBot.Interfaces;
using NotifyBot.Services;

namespace NotifyBot.Modules.MangadexChapter
{
    public class ChaptersMangadex : IIntervalActions
    {
        public SocketTextChannel Channel { get; set; }
        private FrequentChapter _chapter;
        public string Url { get; set; }
        
        public ChaptersMangadex(string url)
        {
            Url = url;
        }

        public ChaptersMangadex(SocketTextChannel channel, string url)
        {
            Channel = channel;
            Url = url;
        }

        public async void StartExecution()
        {
            await GetNewestChapterAsync();
            CheckForChanges();
        }

        public async Task<FrequentChapter> GetNewestChapterAsync()
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

            return _chapter;
        }
        
        private void CheckForChanges()
        {
            var previousChapters = File.ReadAllLines(Paths.ChaptersStorageMangadex);

            for (int i = 0; i < previousChapters.Length; i++)
            {
                if (Url == previousChapters[i].Split(" ")[0])
                {
                    if (_chapter.Number != previousChapters[i].Split(" ")[1] && _chapter.Lang == "English")
                    {
                        SendNotification();
                        previousChapters[i] = $"{Url} {(Convert.ToInt32(previousChapters[i].Split(" ")[1]) + 1)}";
                        File.WriteAllLines(Paths.ChaptersStorageMangadex, previousChapters);
                    }
                }
            }


        }

        private async void SendNotification()
        {
            var embed = new EmbedBuilder
            {
                Title = "New chapter available!",
                Description = $"{_chapter.Number}th chapter of {_chapter.Name} just came out on mangadex.org!"
            };
            var message = embed.WithUrl("https://mangadex.org" + _chapter.Href).Build();
            
            await Channel.SendMessageAsync("", false, message);
        }
    }
}