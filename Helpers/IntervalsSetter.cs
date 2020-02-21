using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Discord.WebSocket;
using NotifyBot.Interfaces;


namespace NotifyBot.Services
{
    public class IntervalActions
    {
        private readonly SocketTextChannel _channel;
        private List<IIntervalActions> _actions;

        public IntervalActions(DiscordSocketClient client)
        {
            ulong serverId = Convert.ToUInt64(File.ReadAllText(Paths.ServerId));
            ulong channelId = Convert.ToUInt64(File.ReadAllText(Paths.ChannelId));
            var channelGetter = new ChannelGetter(serverId, channelId, client);
            
            _channel = channelGetter.GetTextChannel();

            Load();
        }


        private void Load()
        {
            var rnd = new Random();
            var timeout = rnd.Next(7200000, 14400000);
            if (_channel != null)
            {
                Thread.Sleep(timeout);
                var actionsGetter = new IntervalActionsGetter();
                _actions = actionsGetter.GetActions(_channel);
                Send();
            }
        }
        
        private void Send()
        {
            foreach (var action in _actions)
            {
                action.StartExecution();
                Thread.Sleep(20000);
            }
            
            Load();

        }

    }
}