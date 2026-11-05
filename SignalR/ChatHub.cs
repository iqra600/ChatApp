
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
                return;

            // Get logged-in user's ID
            string userId = Context.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get logged-in user's username
            string username = Context.User.Identity.Name;

            // Set sender information from server
            message.UserID = userId;
            message.UserName = username;
            message.When = DateTime.Now;

            // Broadcast message to all connected users
            await Clients.All.SendAsync("msgRecieve", message);
        }
    }
}
