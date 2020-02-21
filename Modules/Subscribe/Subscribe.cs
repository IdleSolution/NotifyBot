using System;
using System.IO;
using System.Threading.Tasks;
using NotifyBot.Modules.ForumPostsMAL;
using NotifyBot.Modules.MangadexChapter;

namespace NotifyBot.Services
{
    public class Subscribe
    {
        public static async Task<string> Mangadex(string link)
        {
            if (!link.Contains("mangadex.org") || !link.Contains("title"))
            {
                return "Wrong link!";
            }
            var subArray = File.ReadAllLines(Paths.SubscribedMangadex);
            foreach (var t in subArray)
            {
                if (t == link)
                {
                    return "You are already subscribed to this manga!";
                }
            }

            var chapters = new ChaptersMangadex(link);
            var frequentChapter = await chapters.GetNewestChapterAsync();
            File.AppendAllText(Paths.ChaptersStorageMangadex, $"{link} {frequentChapter.Number}" + Environment.NewLine);
            File.AppendAllText(Paths.SubscribedMangadex, link + Environment.NewLine);
            return "Manga has been added successfully.";
        }

        public static async Task<string> Mal(string link)
        {
            if (!link.Contains("myanimelist.net/forum/search?u="))
            {
                return "Wrong link!";
            }

            var subArray = File.ReadAllLines(Paths.SubscribedMal);
            foreach (var sub in subArray)
            {
                if (sub == link)
                {
                    return "You are already subscribed to this MAL account!";
                }
            }

            var mal = new ForumPostsMal(link);
            var posts = await mal.GetNewestPostsAsync();
            File.AppendAllText(Paths.SubscribedMal, link);
            
            var str = link + " ";
            foreach (var post in posts)
            {
                str += post.Title.Replace(" ", "") + " ";
            }
            
            File.AppendAllText(Paths.PostsStorageMal, str + Environment.NewLine);

            return "Account has been added successfully";
        }
    }
}