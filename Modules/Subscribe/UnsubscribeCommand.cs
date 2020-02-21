using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace bot.Modules.Subscribe
{
    public class UnsubscribeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("unsubscribe")]
        [Name("unsubscribe <where> <what>")]
        [Summary("Removes subscribed thing")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task RemoveAsync(string place, [Remainder]string link)
        {
            var unsubscribe = new Unsubscribe(link);
            if (place == "mangadex")
            {
                var message = unsubscribe.Mangadex();
                await Context.Channel.SendMessageAsync(message);
            } 
            else if (place == "mal" || place == "myanimelist")
            {
                var message = unsubscribe.Mal();
                await Context.Channel.SendMessageAsync(message);
            }
        }
    }
}