using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Aika_Packet_Sniffer.Model;

namespace Aika_Packet_Sniffer.Network
{
    public class Proxy
    {
        private readonly Action<List<ListViewModel>> _onFinish;
        public bool isRunning;

        private TcpListener _tcpListener;
        private Thread _proxyThread;
        private int conCount;
        private List<ProxyHandler> _handlers;

        public Proxy(Action<List<ListViewModel>> onFinish)
        {
            _onFinish = onFinish;
            conCount = -1;
            isRunning = false;
            _handlers = new List<ProxyHandler>();
            _tcpListener = new TcpListener(IPAddress.Any, 8080);
        }

        public void Start()
        {
            if (isRunning)
            {
                Stop();
                _proxyThread = null;
            }

            try
            {
                _tcpListener.Start();
                isRunning = true;
                _proxyThread = new Thread(ProxyThread);
                _proxyThread.Start();
            }
            catch
            {
                Stop();
            }
        }

        public void Stop()
        {
            isRunning = false;
            foreach (var handler in _handlers)
            {
                handler.Stop();
            }

            _tcpListener.Stop();
            _proxyThread.Abort();
            _proxyThread = null;
        }

        private void ProxyThread()
        {
            while (isRunning)
            {
                try
                {
                    var socket = _tcpListener.AcceptSocket();
                    var handler = new ProxyHandler(socket, conCount++, _onFinish);
                    _handlers.Add(handler);
                    handler.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }
        }
    }
}