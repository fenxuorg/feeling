using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeartRate.API.Models
{
    public class QueryData
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        [JsonProperty("heart_rate")]
        public SortedList<string, int> Data { get; set; }
    }
}