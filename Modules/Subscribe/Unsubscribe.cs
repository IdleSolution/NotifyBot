using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NotifyBot.Services;

namespace bot.Modules.Subscribe
{
    public class Unsubscribe
    {
        private readonly string _link;
        
        public Unsubscribe(string link)
        {
            _link = link;
        }

        public string Mangadex()
        {
            var error = RemoveSubFromTxt(Paths.SubscribedMangadex, Paths.ChaptersStorageMangadex);
            return error ? "Manga with the given link isn't subscribed to the bot!" : "Unsubscribed!";
        }

        public string Mal()
        {
            var error = RemoveSubFromTxt(Paths.SubscribedMal, Paths.PostsStorageMal);
            return error ? "User with the given link isn't subscribed to the bot!" : "Unsubscribed!";
        }

        private bool RemoveSubFromTxt(string subPath, string storagePath)
        {
            var subArray = File.ReadAllLines(subPath);
            var newSubArray = new List<string>();
            var found = false;
            foreach (var t in subArray)
            {
                if (t == _link)
                {
                    found = true;
                    continue;
                }
                newSubArray.Add(t);
            }

            if (!found) return true;
            
            File.WriteAllLines(subPath, newSubArray);

            var storageArr = File.ReadAllLines(storagePath);
            var newStorageArr = new List<string>();
            foreach (var t in storageArr)
            {
                if (t.Split(" ")[0] == _link)
                {
                    continue;
                }
                newStorageArr.Add(t);
            }
            
            File.WriteAllLines(storagePath, newStorageArr);

            return false;
        }
    }
}