using GPS.Server.Services;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Sessions
{
    public class RD07GatewaySession : TcpSession
    {

        private readonly IRD07gatewayListener _rd07gatewayListener;
        private readonly ILogger<RD07GatewaySession> _logger;
        public RD07GatewaySession(
            IServiceProvider serviceProvider,
            TcpServer server
            ) : base(server)
        {
            _rd07gatewayListener = (IRD07gatewayListener)serviceProvider.GetService(typeof(IRD07gatewayListener));
            _logger = (ILogger<RD07GatewaySession>)serviceProvider.GetService(typeof(ILogger<RD07GatewaySession>));
        }

        protected override void OnConnected()
        {
            byte[] utcBytes = System.Text.Encoding.Default.GetBytes(string.Format("@UTC,{0}#", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
            this.Send(utcBytes, 0, utcBytes.Length);
            _logger.LogDebug($"RD07 session with Id {Id} connected!");
        }

        protected override void OnDisconnected()
        {
            _logger.LogDebug($"RD07 session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (size == 0)
                return;

            var _size = Convert.ToInt32(size);

            byte[] recBuf = new byte[size];
            Array.Copy(buffer, recBuf, size);

            try
            {
                //Analysis data
                int serial = _rd07gatewayListener.AckRD07_4G(recBuf);
                //Reply ACK
                if (serial != -1)
                {
                    //Console.WriteLine(serial);
                    byte[] ackBytes = System.Text.Encoding.Default.GetBytes(string.Format("@ACK,{0}#", serial));
                    this.Send(ackBytes, 0, ackBytes.Length);
                    _rd07gatewayListener.Receive(recBuf, _size);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"RD07 session caught an error with code {error}");
        }




    }
}
