using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MITM.Wakfu.Network
{
    public class ServerAuth : ServerCore
    {
        public ClientCore Client { get; set; }
        public ClientCore Server { get; set; }


        public ServerAuth(short port) : base(port)
        {
        }

        protected override void AcceptAsyncEvent_Completed(object sender, SocketAsyncEventArgs e)
        {
            Client = new ClientCore(e.AcceptSocket) { IsServer = false, IsGame = false };
            Server = new ClientCore("52.16.189.225", 5558) { IsServer = true, IsGame = false };

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
