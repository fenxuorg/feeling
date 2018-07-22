using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeartRate.API.Models
{
    public class HeartRate
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        [JsonProperty("heart_rate")]
        public Dictionary<DateTime, int> Data { get; set; }
    }
}