using GPS.Models;
using GPS.Proxy;
using GPS.Redis;
using GPS.Server.Models.RD07DataParser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Services
{
    public interface IRD07WifigatewayListener
    {
        Task Receive(byte[] buffer, int length);
        public int AckRD07Wifi(byte[] data);
        public int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer);
        public UplinkPacket DecodePackage(ref ReadOnlySequence<byte> buffer);
    }
    public class RD07WifigatewayListener : IRD07WifigatewayListener
    {
        private readonly ILogger<RD07WifigatewayListener> _logger;
        private readonly IInventoryProxyAccessor _inventoryProxyAccessor;
        private readonly ICacheService _cacheService;
        private int FixedLength = 6;
        public RD07WifigatewayListener(ILogger<RD07WifigatewayListener> logger,
            IInventoryProxyAccessor inventoryProxyAccessor,
            ICacheService cacheService)
        {
            _logger = logger;
            _inventoryProxyAccessor = inventoryProxyAccessor;
            _cacheService = cacheService;
        }

        public int AckRD07Wifi(byte[] data)
        {
            int serial = -1;
            #region RD07
            string protocols = Encoding.ASCII.GetString(data, 4, 2);
            if (protocols == "$$")
            {
                TZONE.RD07.Protocols.Generic generic = new TZONE.RD07.Protocols.Generic().Analysis(data);
                if (generic != null)
                {
                    serial = Convert.ToInt32(generic.Serial);
                }
            }
            #endregion
            return serial;
        }
        public async Task Receive(byte[] buffer, int length)
        {

            try
            {
                string protocols = Encoding.ASCII.GetString(buffer, 4, 2);

                if (protocols == "$$")
                {
                    TZONE.RD07.Protocols.Generic generic = new TZONE.RD07.Protocols.Generic().Analysis(buffer);
                    //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(generic));
                    RD07Data _RD07Data = new RD07Data();
                    if (generic != null)
                    {
                        //log = "IMEI：" + generic.IMEI + " => Analysis Success";
                        _logger.LogDebug(generic.IMEI + "\n\r" + protocols);

                        if (generic.Tag0708List != null && generic.Tag0708List.Count() > 0)
                        {
                            foreach (var tag in generic.Tag0708List)
                            {
                                _RD07Data.RD07TagList.Add(new RD07Tag()
                                {
                                    SN = long.Parse(tag.SN),
                                    IsLowVoltage = tag.IsLowVoltage,
                                    Humidity = (tag.Humidity * 100) > 120 ? 0 : tag.Humidity,
                                    Temperature = tag.Temperature,
                                    RTC = tag.RTC
                                });
                            }
                        }
                        if (generic.Tag07CList != null && generic.Tag07CList.Count() > 0)
                        {
                            foreach (var tag in generic.Tag07CList)
                            {
                                _RD07Data.RD07TagList.Add(new RD07Tag()
                                {
                                    SN = long.Parse(tag.SN),
                                    IsLowVoltage = tag.IsLowVoltage,
                                    Humidity = 0,
                                    Temperature = tag.Temperature,
                                    RTC = tag.RTC
                                });
                            }
                        }
                        
                        var _imei = long.Parse(generic.IMEI);

                        _RD07Data.IMEI = long.Parse(generic.IMEI);
                       // _RD07Data.GpsDate = generic.ServerTime; //generic.RTC.AddHours(3);
                        _RD07Data.Alram = generic.Alram;
                        _RD07Data.GSMStatus = generic.GSMStatus;
                        _RD07Data.GSMCSQ = generic.GSMCSQ;
                        _RD07Data.HardwareType = generic.HardwareType;

                        //if (_RD07Data.GpsDate > DateTime.Now)
                        //    _RD07Data.GpsDate = DateTime.Now;
                        //Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine(JsonConvert.SerializeObject(_RD07Data));

                        foreach (var tag in _RD07Data.RD07TagList)
                        {
                            var isSensorExists = await _inventoryProxyAccessor.IsSensorExists(tag.SN.ToString());
                            if (!isSensorExists)
                            {
                                _logger.LogWarning($"[RD07] Wifi sensor with serial {tag.SN} was not found");
                                continue;
                            }

                            var inventorySensor = await _inventoryProxyAccessor.GetSensorBySerial(tag.SN.ToString());

                            if (inventorySensor == null)
                            {
                                _logger.LogWarning($"[RD07] Wifi No Invetory for sensor with serial {tag.SN}");
                                continue;
                            }
                            var history = new InventoryHistoryView()
                            {
                                InventoryReferanceKey = inventorySensor.InventoryReferenceKey,
                                InventoryId = inventorySensor.InventoryId,
                                Alram = _RD07Data.Alram,
                                GatewayIMEI = _RD07Data.IMEI.ToString(),
                                GSMStatus = _RD07Data.GSMStatus,
                                Serial = tag.SN.ToString(),
                                GpsDate = tag.RTC.AddMinutes(-30) < DateTime.Now ? tag.RTC.AddHours(3): DateTime.Now, //RD07 wifi Data.GpsDate .AddHours(3),
                                Humidity = tag.Humidity > 0 ? (tag.Humidity * 100) : 0,
                                Temperature = tag.Temperature,
                                IsLowVoltage = tag.IsLowVoltage
                            };

                            

                            await _inventoryProxyAccessor.SaveWarehouseHistory(history);

                            //publish wasl
                            if (!string.IsNullOrWhiteSpace(history.InventoryReferanceKey))
                            {
                                await _inventoryProxyAccessor.HandelWaslPublish(history);
                            }

                        }
                    }
                    else
                    {
                        _logger.LogWarning("RD07 Wifi faild to analyze data");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            await Task.FromResult(0);

        }
        public int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new SequenceReader<byte>(buffer);
            reader.TryRead(out byte header);
            if (header == 0x46)
            {
                //RD05-TAG04
                return 13 - FixedLength;
            }
            else if (header == 0x47)
            {
                //RD05-TAG05
                return 13 - FixedLength;
            }
            else if (header == 0x7F)
            {
                //RD05-TAG06
                return 16 - FixedLength;
            }
            else if (header == 0x7E)
            {
                //RD05-TAG06 新协议,加上湿度
                return 17 - FixedLength;
            }
            else if (header == 0xA0)
            {
                //网关透传协议
                reader.TryReadBigEndian(out short packLength);
                if (packLength <= 1024)
                {
                    //包长度：包长度之后的数据
                    return packLength + 3 - FixedLength; //起始位(1bytes)+包长度(2bytes),再减去固定长度
                }
            }
            else if (header == 0x54)
            {
                reader.TryRead(out byte headerMark);
                if (headerMark == 0x5A)
                {
                    reader.TryReadBigEndian(out short packLength);
                    if (packLength <= 1024)
                    {
                        //包长度：从协议号（包括协议号）到校验位结束的数据
                        return packLength + 2 + 2 + 2 - FixedLength; //起始位(2bytes)+包长度(2bytes)+结束位(2bytes),再减去固定长度
                    }
                }
            }
            return -1;
        }
        public UplinkPacket DecodePackage(ref ReadOnlySequence<byte> buffer)
        {
            var package = new UplinkPacket();
            package.RawMessage = buffer.ToArray();

            var reader = new SequenceReader<byte>(buffer);
            reader.TryRead(out byte header);
            if (header == 0x46 || header == 0x47)
            {
                package.Header = header.ToString().ToUpper();
                package.CodecsName = "Standard";
                package.CodecsType = "TAG";
                if (buffer.Slice(buffer.Length - 1, 1).ToArray()[0] == 0x03)
                {
                    package.CRC = 0x03;
                    package.OK = true;
                }

            }
            else if (header == 0x7F || header == 0x7E)
            {
                package.Header = header.ToString().ToUpper();
                package.CodecsName = "Standard";
                package.CodecsType = "TAG";
                byte[] check_data = buffer.Slice(0, buffer.Length - 2).ToArray();
                byte[] check_crc = buffer.Slice(buffer.Length - 2, 1).ToArray();
                if (CRC.CumulativeChecksum(check_data) == (int)check_crc[0])
                {
                    package.CRC = check_crc[0];
                    package.OK = true;
                }
            }
            else if (header == 0xA0)
            {
                reader.TryReadBigEndian(out short packLength);
                reader.TryRead(out byte headerMark);
                if (headerMark >= 0x00 && headerMark <= 0x06)
                {
                    package.Header = header.ToString().ToUpper();
                    package.CodecsName = "TTP";
                    string commandTypeId = headerMark.ToString().PadLeft(2, '0').ToUpper();

                    if (commandTypeId == "00")
                        package.CodecsType = "HardwareInfo";
                    else if (commandTypeId == "01")
                        package.CodecsType = "Heartbeat";
                    else if (commandTypeId == "02" || commandTypeId == "03")
                        package.CodecsType = "Payload";
                    else if (commandTypeId == "04" || commandTypeId == "05")
                        package.CodecsType = "CMD";
                    else if (commandTypeId == "06")
                        package.CodecsType = "HardwareVersion";

                    if (!string.IsNullOrWhiteSpace(package.CodecsType))
                    {
                        //校验码：从协议号（包括协议号）到校验位之前数据
                        byte[] check_data = buffer.Slice(0, buffer.Length - 2).ToArray();
                        byte[] check_crc = buffer.Slice(buffer.Length - 2, 2).ToArray();
                        if (CRC.Check(check_data, check_crc))
                        {
                            package.CRC = BitConverter.ToInt16(check_crc, 0);
                            package.OK = true;
                        }
                    }
                }
            }
            else if (header == 0x54)
            {
                reader.TryRead(out byte headerMark);
                if (headerMark == 0x5A)
                {
                    package.Header = BitConverter.ToString(buffer.Slice(0, 2).ToArray()).Replace("-", "").ToUpper();
                    reader.TryReadBigEndian(out short packLength);
                    string commandTypeId = BitConverter.ToString(buffer.Slice(4, 2).ToArray()).Replace("-", "");

                    package.CodecsName = "Default";
                    if (commandTypeId == "0000")
                        package.CodecsType = "Login";
                    else if (commandTypeId == "0001")
                        package.CodecsType = "Ping";
                    else if (commandTypeId == "0002")
                        package.CodecsType = "CMD";
                    else if (commandTypeId == "0003")
                        package.CodecsType = "Heartbeat";
                    else if (commandTypeId == "0004")
                        package.CodecsType = "Payload";

                    if (string.IsNullOrWhiteSpace(package.CodecsType))
                    {
                        package.CodecsName = "Standard";
                        if (commandTypeId == "2444")
                            package.CodecsType = "CMD";
                        else if (commandTypeId == "2424" || commandTypeId == "2324" || commandTypeId == "2452") //2324 = TT18C 支持多基站协议  2452 == RD06 $R请求RTC时间
                            package.CodecsType = "Payload";
                    }

                    if (!string.IsNullOrWhiteSpace(package.CodecsType))
                    {
                        //校验码：从协议号（包括协议号）到校验位之前数据
                        byte[] check_data = buffer.Slice(4, packLength - 2).ToArray();
                        byte[] check_crc = buffer.Slice(buffer.Length - 4, 2).ToArray();
                        if (CRC.Check(check_data, check_crc))
                        {
                            package.CRC = BitConverter.ToInt16(check_crc, 0);
                            package.OK = true;
                        }
                    }
                }
            }

            return package;
        }
       
    }
    public class UplinkPacket
    {
        public string Header { get; set; }
        public string CodecsName { get; set; }
        public string CodecsType { get; set; }
        public byte[] RawMessage { get; set; }
        public Int16 CRC { get; set; }
        public bool OK { get; set; } = false;
    }
    public class CRC
    {
        /// <summary>
        /// CRC长度
        /// </summary>
        private const int CRC_LEN = 2;

        /// <summary>
        /// Table of CRC values for high-order byte
        /// CRC高位字节表
        /// </summary>
        private readonly byte[] _auchCRCHi = new byte[]
        {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
        };

        /// <summary>
        /// Table of CRC values for low-order byte
        /// CRC低位字节表
        /// </summary>
        private readonly byte[] _auchCRCLo = new byte[]
        {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
            0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
            0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
            0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
            0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
            0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
            0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
            0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
            0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
            0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
            0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
            0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
            0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
            0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
            0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
            0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
            0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
            0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };

        /// <summary>
        /// CRC16位计算检验
        /// </summary>
        /// <param name="buffer">校验byte数组</param>
        /// <returns>返回的是一个ushort类型的值</returns>
        public ushort CalculateCrc16(byte[] buffer)
        {
            string str = ToHexString(buffer);
            str = str.Replace(" ", "");
            //HexCon.StringToByte(str);
            byte[] b = Encoding.ASCII.GetBytes(str);
            byte crcHi = 0xff;  // high crc byte initialized
            byte crcLo = 0xff;  // low crc byte initialized 
            for (int i = 0; i < b.Length; i++)
            {
                int crcIndex = crcLo ^ b[i]; // calculate the crc lookup index
                crcLo = (byte)(crcHi ^ _auchCRCHi[crcIndex]);
                crcHi = _auchCRCLo[crcIndex];
            }
            return (ushort)(crcHi << 8 | crcLo);
        }

        /// <summary>
        /// CRC16位计算检验
        /// CalculateCrc16返回的是一个ushort类型的值，如果要返回Crc高字节和低字节，可重写CalculateCrc16函数
        /// </summary>
        /// <param name="buffer">校验byte数组</param>
        /// <param name="crcHi">输出高位</param>
        /// <param name="crcLo">输出低位</param>
        /// <returns>返回的是一个ushort类型的值</returns>
        public ushort CalculateCrc16(byte[] buffer, out byte crcHi, out byte crcLo)
        {
            crcHi = 0xff;  // high crc byte initialized
            crcLo = 0xff;  // low crc byte initialized 
            for (int i = 0; i < buffer.Length; i++)
            {
                int crcIndex = crcHi ^ buffer[i]; // calculate the crc lookup index
                crcHi = (byte)(crcLo ^ _auchCRCHi[crcIndex]);
                crcLo = _auchCRCLo[crcIndex];
            }
            return (ushort)(crcHi << 8 | crcLo);
        }

        /// <summary>
        /// CRC16位 Hex检验
        /// </summary>
        /// <param name="buffer">校验byte数组</param>
        /// <returns>返回的是一个ushort类型的值</returns>
        public ushort CalculateCrc16Hex(byte[] buffer)
        {
            byte crcHi = 0xff;  // high crc byte initialized
            byte crcLo = 0xff;  // low crc byte initialized 
            for (int i = 0; i < buffer.Length; i++)
            {
                int crcIndex = crcHi ^ buffer[i]; // calculate the crc lookup index
                crcHi = (byte)(crcLo ^ _auchCRCHi[crcIndex]);
                crcLo = _auchCRCLo[crcIndex];
            }
            return (ushort)(crcHi | crcLo << 8);
        }

        /// <summary>
        /// byte数组转换成2位十六进制
        /// </summary>
        /// <param name="bytes">传入byte数组</param>
        /// <returns>返回一个字符串</returns>
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString">转换字符</param>
        /// <returns>返回转换后字符数组</returns>
        public static byte[] GetStringToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// 获取校验CRC码
        /// </summary>
        /// <param name="obj">校验字符</param>
        /// <returns></returns>
        public static string GetCRC(string obj)
        {
            byte[] b = GetStringToToHexByte(obj);
            CRC crc = new CRC();
            ushort crc16byte = crc.CalculateCrc16Hex(b);
            byte[] b1 = new byte[2];
            b1[0] = (byte)((0xff00 & crc16byte) >> 8);
            b1[1] = (byte)(0xff & crc16byte);
            string H1 = ToHexString(b1);
            byte[] H2 = Encoding.UTF8.GetBytes(H1);
            string ATH = ToHexString(H2);
            return ATH;
        }
        /// <summary>
        /// 获取校验CRC码
        /// </summary>
        /// <param name="buffer">校验字符</param>
        /// <returns></returns>
        public static byte[] GetCRC(byte[] buffer)
        {
            CRC crc = new CRC();
            ushort crc16byte = crc.CalculateCrc16Hex(buffer);
            byte[] b1 = new byte[2];
            b1[0] = (byte)((0xff00 & crc16byte) >> 8);
            b1[1] = (byte)(0xff & crc16byte);
            return b1;
        }

        /// <summary>
        /// 检查校验CRC,是否正确
        /// </summary>
        /// <param name="buffer">校验字符</param>
        /// <param name="check">校验码</param>
        /// <returns></returns>
        public static bool Check(byte[] buffer, byte[] check)
        {
            byte[] m_buffer = GetCRC(buffer);
            string m_buffer_string = ToHexString(m_buffer);
            string m_check_string = ToHexString(check);
            if (m_buffer_string.Equals(m_check_string))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取累加校验和
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static int CumulativeChecksum(byte[] buffer)
        {
            int num = 0;
            for (int i = 0; i < buffer.Length; i++)
                num = (num + buffer[i]) % 0xffff;
            byte[] bytes = BitConverter.GetBytes(num);
            return (int)bytes[0];
        }

    }
}
