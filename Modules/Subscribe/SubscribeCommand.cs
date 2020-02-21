using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using NotifyBot.Services;

namespace NotifyBot.Modules
{
    public class SubscribeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("subscribe")]
        [Name("Subscribe <where> <what>")]
        [Summary("Adds a new thing to check for")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task AddAsync(string place, [Remainder]string link)
        {
            if (place == "mangadex")
            {
                var message = await Subscribe.Mangadex(link);
                await Context.Channel.SendMessageAsync(message);
            } 
            else if (place == "mal" || place == "myanimelist")
            {
                var message = await Subscribe.Mal(link);
                await Context.Channel.SendMessageAsync(message);
            }
        }
    }
}