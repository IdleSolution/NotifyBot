using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NotifyBot.Services;

namespace NotifyBot
{
    public class Program
    {
        private readonly string _token = File.ReadAllText(@"C:\Users\idles\Desktop\NotifyBot\token.txt");
        private DiscordSocketClient _client;
        private CommandHandler _handler;
        
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {

            _client = new DiscordSocketClient();
            _client.Log += Log;

            _handler = new CommandHandler(_client);

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            
            Thread.Sleep(8000);
            new IntervalActions(_client);

            await Task.Delay(-1);

        }
        
    }
}