using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using Aika_Packet_Sniffer.Logger;
using Aika_Packet_Sniffer.Model;

namespace Aika_Packet_Sniffer.Network
{
    public class ProxyHandler
    {
        private readonly Socket _recvSocket;
        private readonly Thread _recvThread;
        private Socket _sendSocket;
        private Thread _sendThread;
        private int _connectionId;
        private readonly LogWrite _logWriter;
        private ushort _port;

        public ProxyHandler(Socket socket, int conId, Action<List<ListViewModel>> onFinish)
        {
            _recvSocket = socket;
            _connectionId = conId;
            _recvThread = new Thread(RecvThread);
            _logWriter = new LogWrite(onFinish);
        }

        private void RecvThread()
        {
            try
            {
                if (ReceiveBytes(1)[0] != 4)
                {
                    Shutdown();
                    return;
                }
            }
            catch (Exception)
            {
                Shutdown();
                return;
            }

            if (ReceiveBytes(1)[0] != 1) throw new Exception();
            var p1 = ReceiveBytes(1)[0];
            var p2 = ReceiveBytes(1)[0];
            var port = BitConverter.ToUInt16(new[] {p2, p1}, 0);
            var ip = BitConverter.ToUInt32(ReceiveBytes(4), 0);
            while (ReceiveBytes(1)[0] != 0)
            {
            }

            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(new IPAddress(ip), port);
                _port = port;
            }
            catch (Exception)
            {
                _recvSocket.Send(new byte[] {0, 91, 0, 0, 0, 0, 0, 0});
                Shutdown();
                return;
            }

            try
            {
                _recvSocket.Send(new byte[] {0, 90, 0, 0, 0, 0, 0, 0});
                _sendSocket = tcpClient.Client;
                _sendThread = new Thread(SendThread);
                _sendThread.Start();

                while (true)
                {
                    var stream = new byte[15000];
                    var a = _sendSocket.Receive(stream);
                    var buff = new byte[a];
                    Buffer.BlockCopy(stream, 0, buff, 0, a);
                    _recvSocket.Send(buff, a, SocketFlags.None);
                    _logWriter.WritePacket(buff, PacketOrigin.ServerToClient, _port);

                    if (a == 0) break;
                }

                _recvSocket.Shutdown(SocketShutdown.Receive);
                _sendSocket.Shutdown(SocketShutdown.Send);
                Shutdown(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private void SendThread()
        {
            try
            {
                while (true)
                {
                    var stream = new byte[15000];
                    var a = _recvSocket.Receive(stream);
                    var buff = new byte[a];
                    Buffer.BlockCopy(stream, 0, buff, 0, a);
                    _sendSocket.Send(buff, a, SocketFlags.None);
                    _logWriter.WritePacket(buff, PacketOrigin.ClientToServer, _port);

                    if (a == 0) break;
                }

                _recvSocket.Shutdown(SocketShutdown.Send);
                _sendSocket.Shutdown(SocketShutdown.Receive);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private byte[] ReceiveBytes(int count)
        {
            var buffer = new byte[count];

            var i = 0;
            while (i != count)
            {
                var p = _recvSocket.Receive(buffer, i, count - i, SocketFlags.None);
                i += p;
            }

            return buffer;
        }

        private void Shutdown(bool recv = true)
        {
            if (recv)
            {
                _recvSocket.Shutdown(SocketShutdown.Both);
                _recvSocket.Close();
            }
            else
            {
                _sendThread.Join();
            }
        }

        public void Start()
        {
            _recvThread.Start();
        }

        public void Stop()
        {
            try
            {
                _recvThread.Join();
                _sendThread.Join();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}