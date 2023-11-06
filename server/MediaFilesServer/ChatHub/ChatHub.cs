using Microsoft.AspNetCore.SignalR;

namespace FRServer.FrHub
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
            //await Clients.All.SendAsync("messageReceived", username, message);
        }
    }
}
