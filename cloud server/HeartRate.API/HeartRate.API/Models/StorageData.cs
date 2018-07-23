using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeartRate.API.Models
{
    public class StorageData
    {
        [JsonProperty("data")]
        public SortedList<string, int> Data { get; set; }
    }
}