using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ServerClientCommon;

namespace Server
{
    public class ServerHub : Hub<IClient>,IServer
    {
        public bool BroadcastMessage(string message)
        {
            Clients.All.LogMessage(message);
            return true;
        }

    }
}