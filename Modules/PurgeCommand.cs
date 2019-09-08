using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace NotifyBot.Modules
{
    public class PurgeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("purge")]
        [Name("purge <amount>")]
        [Summary("Deletes a specified amount of messages")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DelMesAsync(int delnum)
        {
            var messages = await Context.Channel.GetMessagesAsync(delnum).FlattenAsync();
            await ((ITextChannel) Context.Channel).DeleteMessagesAsync(messages);
        } 
    }
}