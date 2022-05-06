using Microsoft.AspNetCore.SignalR;

namespace SignalRFirst.Hubs
{
    public class PositionHub : Hub
    {
        public async Task SendPosition(int left, int top)
        {
            Console.WriteLine(left + " - " + top);
            if(left < 0)
            {
                left = 0;
            }
            if (top < 0)
            {
                top = 0;
            }
            await Clients.Others.SendAsync("ReceivePosition", left, top);
        }

    }
}
