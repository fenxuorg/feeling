using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeartRate.API.Models
{
    public class ReceivedData
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        // {yyyyMMdd_HHmmss_fff, beats}
        [JsonProperty("heart_rate")]
        public Dictionary<string, int> Data { get; set; }
    }
}