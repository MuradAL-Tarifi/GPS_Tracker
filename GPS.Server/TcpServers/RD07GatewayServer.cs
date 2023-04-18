﻿using GPS.Server.Sessions;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.TcpServers
{
    public class RD07GatewayServer : TcpServer
    {
        private readonly IServiceProvider _serviceProvider;

        public RD07GatewayServer(IPAddress address, int port, IServiceProvider serviceProvider) : base(address, port)
        {
            _serviceProvider = serviceProvider;
        }

        protected override TcpSession CreateSession()
        {
            return new RD07GatewaySession(_serviceProvider, this);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"[RD07GatewayServer] TCP server caught an error with code {error}");
        }
    }
}
