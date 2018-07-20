using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.OPS.NotificationService.WebApi.Models
{
    /// <summary>
    /// The request for sending event.
    /// </summary>
    public class SendEventRequest
    {
        /// <summary>
        /// Gets or sets the name of event.
        /// It should be EventType, such as "BuildJobQueued", "BuildJobStarted", "BuildJobCompleted", "SyncJobQueued", "SyncJobStarted", "SyncJobCompleted", "XRefMapFilePublished",
        /// "XRefMapFileDeleted", "DocsetGoLiveTriggered", "DocsetGoOfflineTriggered", "DocsetGoLiveCompleted", "DocsetGoOfflineCompleted", "DocsetRedirectTriggered", "DocsetRedirectCompleted", "BranchDeleted", etc...
        /// It can not be null or emtpy.
        /// </summary>
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets partition Key.
        /// Indicate message with a partitionKey are guaranteed to land on the same partition.
        /// It can be null or empty.
        /// </summary>
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// It can be type of json object example as {"source_repository_info":"https://github.com/example/example.git","source_branch":"master","priority":"High"}.
        /// It can be any other types for string "example" or array ["example"] etc...
        /// It can be null or empty.
        /// </summary>
        [JsonProperty("message")]
        public JToken Message { get; set; }
    }
}
