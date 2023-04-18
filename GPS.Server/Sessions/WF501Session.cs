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
    public class WF501Session : TcpSession
    {

        private readonly IWF501Listener _WF501Listener;
        private readonly ILogger<WF501Session> _logger;
        public WF501Session(
            IServiceProvider serviceProvider,
            TcpServer server
            ) : base(server)
        {
            _WF501Listener = (IWF501Listener)serviceProvider.GetService(typeof(IWF501Listener));
            _logger = (ILogger<WF501Session>)serviceProvider.GetService(typeof(ILogger<WF501Session>));
        }

        protected override void OnConnecting()
        {
            _logger.LogDebug($"WF501 started connnting with Id {Id}");
        }
        protected override void OnConnected()
        {
            byte[] utcBytes = System.Text.Encoding.Default.GetBytes(string.Format("@UTC,{0}#", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
            //byte[] utcBytes = System.Text.Encoding.Default.GetBytes(string.Format("@utc,{0}#", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            this.Send(utcBytes, 0, utcBytes.Length);
            _logger.LogDebug($"WF501 session with Id {Id} connected!");
        }

        protected override void OnDisconnected()
        {
            _logger.LogDebug($"WF501 session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (size == 0)
                return;

            try
            {
                var _size = Convert.ToInt32(size);

                byte[] recBuf = new byte[size];
                Array.Copy(buffer, recBuf, size);

                //Analysis data
                int serial = _WF501Listener.AckWF501(recBuf);
                //Reply ACK
                if (serial != -1)
                {
                    //Console.WriteLine(serial);
                    byte[] ackBytes = System.Text.Encoding.Default.GetBytes(string.Format("@ACK,{0}#", serial));
                    this.Send(ackBytes, 0, ackBytes.Length);
                    _WF501Listener.Receive(recBuf, _size);
                }
                //if (this.IsConnected)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"WF501 session caught an error with code {error}");
        }

    }

}
