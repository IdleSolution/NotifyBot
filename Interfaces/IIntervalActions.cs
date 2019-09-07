using Discord.WebSocket;

namespace NotifyBot.Interfaces
{
    public interface IIntervalActions
    {
        void StartExecution();
        SocketTextChannel Channel { get; set; }
        string Url { get; set; }
    }
}