using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Unary.RCONClient
{
    public class Client
    {
        public enum ConsoleColors : byte
        {
            Message,
            Warning,
            Error
        };

        private EventBasedNetListener Listener;
        private NetManager NetClient;

        public Client()
        {
            Listener = new EventBasedNetListener();

            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
            Listener.NetworkErrorEvent += OnNetworkError;

            NetClient = new NetManager(Listener);
            NetClient.Start();
        }

        private void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            Program.Running = false;
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Program.Running = false;
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            ConsoleColors CurrentColor = (ConsoleColors)reader.GetByte();

            switch(CurrentColor)
            {
                case ConsoleColors.Message:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ConsoleColors.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ConsoleColors.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(Encoding.UTF8.GetString(reader.GetRemainingBytes()));

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Send(string Command)
        {
            NetDataWriter Writer = new NetDataWriter();
            Writer.Put(Encoding.UTF8.GetBytes(Command));
            NetClient.FirstPeer.Send(Writer, DeliveryMethod.ReliableOrdered);
        }

        public void Start(string IP, int Port, string Password)
        {
            NetClient.Connect(IP, Port, Password);
        }
        
        public void Poll()
        {
            NetClient.PollEvents();
        }

        public void Stop()
        {
            NetClient.Stop(true);
        }
    }
}
