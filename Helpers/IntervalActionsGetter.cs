using System.Collections.Generic;
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
            var allActions = new List<IIntervalActions>()
            {
                new ForumPosts(channel, "https://myanimelist.net/forum/search?u=IdleSolution&q=&uloc=1&loc=-1"),
                new Chapters(channel,
                    "https://mangadex.org/title/17274/kaguya-sama-wa-kokurasetai-tensai-tachi-no-renai-zunousen"),
                new Chapters(channel, "https://mangadex.org/title/21843/karakai-jouzu-no-moto-takagi-san"),
                new Chapters(channel, "https://mangadex.org/title/13108/karakai-jouzu-no-takagi-san")

            };

            return allActions;
        }
    }
}