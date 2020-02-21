using System.Collections.Generic;
using System.IO;
using Discord.WebSocket;
using NotifyBot.Interfaces;
using NotifyBot.Modules.ForumPostsMAL;
using NotifyBot.Modules.MangadexChapter;

namespace NotifyBot.Services
{
    public class IntervalActionsGetter
    {
        public List<IIntervalActions> GetActions(SocketTextChannel channel)
        {
            var allActions = new List<IIntervalActions>();

            MangadexActions(channel, allActions);
            MalActions(channel, allActions);

            return allActions;
        }

        private void MangadexActions(SocketTextChannel channel, ICollection<IIntervalActions> list)
        {
            var subscribedArr = File.ReadAllLines(Paths.SubscribedMangadex);

            foreach (var sub in subscribedArr)
            {
                list.Add(new ChaptersMangadex(channel, sub));
            }
        }
        
        private void MalActions(SocketTextChannel channel, ICollection<IIntervalActions> list)
        {
            var subscribedArr = File.ReadAllLines(Paths.SubscribedMal);

            foreach (var sub in subscribedArr)
            {
                list.Add(new ForumPostsMal(channel, sub));
            }
        }
    }
}