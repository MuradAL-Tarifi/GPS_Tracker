using GPS.Server.Services;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Sessions
{
    public class RD07WifiGatewaySession : TcpSession
    {

        private readonly IRD07WifigatewayListener _rd07WifigatewayListener;
        private readonly ILogger<RD07WifiGatewaySession> _logger;
        public RD07WifiGatewaySession(
            IServiceProvider serviceProvider,
            TcpServer server
            ) : base(server)
        {
            _rd07WifigatewayListener = (IRD07WifigatewayListener)serviceProvider.GetService(typeof(IRD07WifigatewayListener));
            _logger = (ILogger<RD07WifiGatewaySession>)serviceProvider.GetService(typeof(ILogger<RD07WifiGatewaySession>));
        }


        protected override void OnConnected()
        {
            byte[] utcBytes = System.Text.Encoding.Default.GetBytes(string.Format("@UTC,{0}#", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
            this.Send(utcBytes, 0, utcBytes.Length);
            _logger.LogDebug($"RD07 Wifi session with Id {Id} connected!");
        }

        protected override void OnDisconnected()
        {
            _logger.LogDebug($"RD07 Wifi session with Id {Id} disconnected!");
        }
      protected Dictionary<string, byte[]> packets = new Dictionary<string, byte[]>();
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (size == 0)
                return;
            try
            {
                string key = string.Empty;
                var _size = Convert.ToInt32(size);
                byte[] recBuf = new byte[size];
                Array.Copy(buffer, recBuf, size);
                //Console.WriteLine(BitConverter.ToString(recBuf).Replace("-", ""));
                ReadOnlySequence<byte> readOnlyBuffer = new ReadOnlySequence<byte>(recBuf);
                var bodyLength = _rd07WifigatewayListener.GetBodyLengthFromHeader(ref readOnlyBuffer);
                if ((bodyLength + 6) > size)
                {
                    key = Id + "{" + (bodyLength + 6 - size).ToString() + "}";
                    if (!packets.ContainsKey(key))
                    {
                        packets.Add(key, recBuf);
                        //Console.WriteLine("");
                        return;
                    }
                }
                else
                {
                    if (bodyLength == -1)
                    {
                        key = Id + "{" + size.ToString() + "}";
                        byte[] packet;
                        var hasData = packets.TryGetValue(key, out packet);
                        if (hasData && packet != null)
                        {
                            var tempListByte = new List<byte>();
                            tempListByte.AddRange(packet);
                            tempListByte.AddRange(recBuf);
                            recBuf = Array.Empty<byte>();
                            recBuf = new byte[tempListByte.Count];
                            recBuf = tempListByte.ToArray();
                            packets.Remove(key);
                            readOnlyBuffer = new ReadOnlySequence<byte>(recBuf);
                        }
                    }
                    //Console.WriteLine("");
                    var decodePackage = _rd07WifigatewayListener.DecodePackage(ref readOnlyBuffer);
                    //Console.WriteLine(BitConverter.ToString(decodePackage.RawMessage).Replace("-", ""));
                  
                    if (decodePackage.OK)
                    {
                        //Analysis data
                        int serial = _rd07WifigatewayListener.AckRD07Wifi(recBuf);
                        //Reply ACK
                        if (serial != -1)
                        {
                            //Console.WriteLine(serial);
                            byte[] ackBytes = System.Text.Encoding.Default.GetBytes(string.Format("@ACK,{0}#", serial));
                            //_logger.LogInformation("rd07Wifi: {" + System.Text.Encoding.UTF8.GetString(ackBytes) + "}");
                            this.Send(ackBytes, 0, ackBytes.Length);
                            _rd07WifigatewayListener.Receive(recBuf, _size);
                        }
                    }
                    else
                    {
                        _logger.LogError("failed to decode package: " + BitConverter.ToString(decodePackage.RawMessage).Replace("-", "")); 
                    }
                    //Console.WriteLine("session id: " + Id);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"RD07 Wifi session caught an error with code {error}");
        }
       
        
      
    }
    
}
