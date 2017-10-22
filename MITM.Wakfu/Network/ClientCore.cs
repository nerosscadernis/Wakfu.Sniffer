using MITM.Wakfu.Extensions;
using MITM.Wakfu.Timers;
using MITM.Wakfu.Utils;
using Rebirth.Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MITM.Wakfu.Network
{
    public class ClientCore : IDisposable
    {
        #region MyRegion
        const int bufferLength = 8192;

        public byte[] buffer = new byte[bufferLength];
        public Socket Socket;

        public bool IsServer { get; set; }
        public bool IsGame { get; set; }

        protected SocketAsyncEventArgs ReceiveEvent;
        protected event Action Disconnected;

        private TimerCore _timer;
        private object _sender;
        protected object _receiver;
        #endregion

        #region Constructor
        public ClientCore(Socket socket)
        {
            _sender = new object();
            _receiver = new object();
            Socket = socket;
            socket.NoDelay = true;

            ReceiveEvent = new SocketAsyncEventArgs();
            ReceiveEvent.SetBuffer(buffer, 0, buffer.Length);
            ReceiveEvent.Completed += ReceiveEvent_Completed;

            _timer = new TimerCore(new Action(CheckDisonnect), 50, 50);

            if (!Socket.ReceiveAsync(ReceiveEvent))
            {
                ReceiveEvent_Completed(Socket, ReceiveEvent);
            }
        }

        public ClientCore(string ip, short port)
        {
            _sender = new object();
            _receiver = new object();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            e.UserToken = Socket;
            e.Completed += new EventHandler<SocketAsyncEventArgs>(e_Completed);
            Socket.ConnectAsync(e);
        }

        protected virtual void e_Completed(object sender, SocketAsyncEventArgs e)
        {
            ReceiveEvent = new SocketAsyncEventArgs();
            ReceiveEvent.SetBuffer(buffer, 0, buffer.Length);
            ReceiveEvent.Completed += ReceiveEvent_Completed;         

            _timer = new TimerCore(new Action(CheckDisonnect), 50, 50);

            if (!Socket.ReceiveAsync(ReceiveEvent))
            {
                ReceiveEvent_Completed(Socket, ReceiveEvent);
            }
        }

        #endregion

        #region Event
        protected virtual void ReceiveEvent_Completed(object sender, SocketAsyncEventArgs e)
        {
            do
            {
                if (!IsConnected())
                    break;
                var reader = new BigEndianReader(buffer.SubArray(0, ReceiveEvent.BytesTransferred));
                while (reader.BytesAvailable > 0)
                {
                    var msg = new NetworkMessage();
                    msg.Read(reader, !IsServer);
                    if (!IsGame)
                    {
                        if (IsServer)
                        {
                            if (msg.Id == 1036)
                            {
                                List<ServerType> servers = new List<ServerType>();
                                var q = reader.ReadInt();
                                for (int i = 0; i < q; i++)
                                {
                                    var serv = new ServerType();
                                    serv.Ports = new List<int>();
                                    serv.Id = reader.ReadInt();
                                    serv.Name = reader.ReadUTF();
                                    serv.Commu = reader.ReadInt();
                                    reader.ReadUTF();
                                    serv.Ip = "127.0.0.1";
                                    var countPort = reader.ReadInt();
                                    for (int z = 0; z < countPort; z++)
                                    {
                                        serv.Ports.Add(reader.ReadInt());
                                    }
                                    serv.Position = reader.ReadByte();
                                    servers.Add(serv);
                                }
                                var writer = new BigEndianWriter();
                                writer.WriteUShort(1036);
                                writer.WriteInt(servers.Count);
                                foreach (var item in servers)
                                {
                                    writer.WriteInt(item.Id);
                                    writer.WriteUTF(item.Name);
                                    writer.WriteInt(item.Commu);
                                    writer.WriteUTF(item.Ip);
                                    writer.WriteInt(item.Ports.Count);
                                    foreach (var port in item.Ports)
                                    {
                                        writer.WriteInt(port);
                                    }
                                    writer.WriteByte(item.Position);
                                }
                                var bytes = reader.ReadBytes((int)reader.BytesAvailable);
                                writer.WriteBytes(bytes);
                                var writer2 = new BigEndianWriter();
                                writer2.WriteUShort((ushort)(writer.Data.Length + 2));
                                writer2.WriteBytes(writer.Data);
                                Program.ServerLogin.SendToClient(writer2.Data);
                                LogManager.GetLoggerClass().Server(string.Format("Id [{0}] | Data : {1}", msg.Id, writer2.Data.GetDatas(Program.Hexa)));
                            }
                            else
                            {
                                Program.ServerLogin.SendToClient(msg.CompleteDatas);
                                LogManager.GetLoggerClass().Server(string.Format("Id [{0}] | Data : {1}", msg.Id, msg.Datas.GetDatas(Program.Hexa)));
                            }
                        }
                        else
                        {
                            Program.ServerLogin.SendToServer(msg.CompleteDatas);
                            LogManager.GetLoggerClass().Client(string.Format("Id [{0}] | Data : {1}", msg.Id, msg.Datas.GetDatas(Program.Hexa)));
                        }
                    }
                    else
                    {
                        if (IsServer)
                        {
                            Program.ServerGame.SendToClient(msg.CompleteDatas);
                            LogManager.GetLoggerClass().Server(string.Format("Id [{0}] | Data : {1}", msg.Id, msg.Datas.GetDatas(Program.Hexa)));
                        }
                        else
                        {
                            Program.ServerGame.SendToServer(msg.CompleteDatas);
                            LogManager.GetLoggerClass().Client(string.Format("Id [{0}] | Data : {1}", msg.Id, msg.Datas.GetDatas(Program.Hexa)));
                        }
                    }
                }
                
            } while (!Socket.ReceiveAsync(ReceiveEvent));
        }

        protected virtual void DisconnectedEvent()
        {
            Disconnected?.Invoke();
            _timer.Dispose();
        }
        #endregion

        #region Funcs
        //public void Send(NetworkMessage msg)
        //{
        //    if (!IsConnected())
        //        return;

        //    var writer = new BigEndianWriter();
        //    MessagePacking pack = new MessagePacking();
        //    pack.Pack(msg, writer);
        //    LogManager.GetLoggerClass().Send(msg.ToString().Split('.').Last());

        //    lock (_sender)
        //    {
        //        Socket.Send(writer.Data);
        //    }
        //}

        public void Send(byte[] datas)
        {
            if (!IsConnected())
                return;

            lock (_sender)
            {
                Socket.Send(datas);
            }
        }

        public virtual void Dispose()
        {
            Socket.Dispose();
            Socket = null;
            buffer = null;
            _timer.Dispose();
            Disconnected?.Invoke();
        }

        private void CheckDisonnect()
        {
            if (!IsConnected())
                DisconnectedEvent();
        }

        public bool IsConnected()
        {
            if (Socket == null)
                return false;
            else
                return Socket.IsConnected();
        }
        #endregion
    }
}
