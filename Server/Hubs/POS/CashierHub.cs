using Microsoft.AspNetCore.SignalR;

namespace WebApp.Hubs.POS
{
    public class CashierHub : Hub
    {
        public async Task Send_LoadRoomTable(string POSCode, string roomTableID, string userID)
        {
            await Clients.All.SendAsync("Receive_LoadRoomTable", POSCode, roomTableID, userID);
        }
    }
}
