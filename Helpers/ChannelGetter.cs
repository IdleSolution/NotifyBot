using Discord.WebSocket;

namespace NotifyBot.Services
{
    public class ChannelGetter
    {
        private readonly ulong _serverId;
        private readonly ulong _channelId;
        private readonly DiscordSocketClient _client;
        
        public ChannelGetter(ulong serverId, ulong channelId, DiscordSocketClient client)
        {
            _serverId = serverId;
            _channelId = channelId;
            _client = client;
        }
        
        public SocketTextChannel GetTextChannel()
        {
            var guild = _client.GetGuild(_serverId);
            var channel = guild.GetTextChannel(_channelId);
            return channel;
        }

    }
}