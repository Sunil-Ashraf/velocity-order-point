using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace OrderPoint.API.Hubs
{
  
    
    [AllowAnonymous]
    public class UserHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            // Optional: log connection
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"User connected: {Context.UserIdentifier}");
            return base.OnConnectedAsync();
        }
    }
}
