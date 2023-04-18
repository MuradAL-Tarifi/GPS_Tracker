using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Models.Qc_Moko107Parser
{
    public class Moko107Data
    {
        public int msg_id { get; set; }
        public DeviceInfo device_info { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Data>))]
        [JsonProperty("data")]
        public List<Data> sensors { get; set; }
    }
    public class DeviceInfo
    {
        public string device_id { get; set; }
        public string mac { get; set; }
    }
    public class Data
    {
        public int type { get; set; }
        public Value value { get; set; }
        public string net_state { get; set; }
    }
    public class Value
    {
        public DateTime timestamp { get; set; }
        public string type { get; set; }
        public string mac { get; set; }
        public int rssi { get; set; }
        public int tx_power { get; set; }

        [JsonProperty("rssi@0m")]
        public int Rssi0m { get; set; }
        public int adv_interval { get; set; }
        public int battery_voltage { get; set; }
        public string temperature { get; set; }
        public string humidity { get; set; }
    }
    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
