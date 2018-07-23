using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeartRate.API.Models
{
    public class ReceivedData
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        [JsonProperty("heart_rate")]
        public Dictionary<string, int> Data { get; set; }
    }
}