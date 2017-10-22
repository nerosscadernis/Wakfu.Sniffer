using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MITM.Wakfu.Network
{
    public class ServerGame : ServerCore
    {
        public ClientCore Client { get; set; }
        public ClientCore Server { get; set; }


        public ServerGame(short port) : base(port)
        {
        }

        protected override void AcceptAsyncEvent_Completed(object sender, SocketAsyncEventArgs e)
        {
            Server = new ClientCore("52.50.167.81", 5556) { IsServer = true, IsGame = true };
            Client = new ClientCore(e.AcceptSocket) { IsServer = false, IsGame = true };

            base.AcceptAsyncEvent_Completed(sender, e);
        }

        public void SendToServer(byte[] buffer)
        {
            Server.Send(buffer);
        }

        public void SendToClient(byte[] buffer)
        {
            Client.Send(buffer);
        }

        public void RemoveClient(ClientCore client)
        {
            Client = null;
        }
    }
}
