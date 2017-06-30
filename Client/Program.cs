using Microsoft.AspNet.SignalR.Client;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeSafeSignalR.Common;

namespace Client
{
    class Program
    {

        public class ClientImplementation : IClient
        {
            public void LogMessage(string message)
            {
                Console.WriteLine(message);
            }
        }

        static void Main(string[] args)
        {
            var hubConnection = new HubConnection("http://localhost:50636/signalr");
            var proxy = hubConnection.CreateHubProxy("ServerHub");
            
            //this is the client listener
            IClient cl = new ClientImplementation();
            
            //subscriptions are here
            ClientHubSubscriber<IClient>.CreateSubs(proxy, cl);

            //this is the server calls interface mapping
            IServer typeSafeProxyWrapper = ServerHubProxyHandler<IServer>.Create(proxy);

            hubConnection.Start().ContinueWith((t) => 
            {
                if(t.IsCanceled || t.IsFaulted)
                {
                    Environment.Exit(1);
                }
            }).Wait();
            while (true)
            {
                typeSafeProxyWrapper.BroadcastMessage(Console.ReadLine());
            }
        }
    }
}
