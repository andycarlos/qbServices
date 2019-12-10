using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using qbService.Controllers;
using qbService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace qbService.Hubs
{
    public  class ServiceHub:Hub
    {

        public ServiceHub()
        {
            //HttpContext Current = m_httpContextAccessor.HttpContext;
            //AppBaseUrl = $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";
        }

        public async Task SendMessage(string conetionId, string message)
        {
            await Clients.Client(conetionId).SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }
        public  async Task RunQuery(string conetionId, string query)
        {
            await Clients.Client(conetionId).SendAsync("runQuery", query);
        }

        public override async Task OnConnectedAsync()
        {
            //await Clients.All.SendAsync("ReceiveMessage", "Contect",  Context.ConnectionId);
            await Clients.Clients(Context.ConnectionId).SendAsync("Login");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            //await Clients.All.SendAsync("ReceiveMessage", "Disco" , Context.ConnectionId);
            HubUser.HubUsers.RemoveAll(x => x.ConectionId == Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }

 

}
