using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace netcore_api.Models
{
    [Serializable]
    public class SaveSelectionRequestModel: BaseSelectionModel
    {
    }

    [Serializable]
    public class BaseSelectionModel
    {
        [JsonPropertyName("name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "name field is required")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "id field is required")]
        public string Id { get; set; }


        [JsonPropertyName("user_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "user_id field is required")]
        public string UserId { get; set; }
    }


    [Serializable]
    public class SynchronizeSelectionSummaryRequestModel
    {        
        [JsonPropertyName("user_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "user_id field is required")]
        public string UserId { get; set; }
    }

    [Serializable]
    public class BaseSocketRequest
    {

        [JsonPropertyName("id")]
        public string SocketId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }


    [Serializable]
    public class SendSocketMessageRequest : BaseSocketRequest
    {

    }

    [Serializable]
    public class RemoveSocketRequest : BaseSocketRequest
    {

    }


    [Serializable]
    public class SelectionSummaryKafkaMessage
    {
        [JsonPropertyName("name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "name field is required")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        
        public int Count { get; set; }


        [JsonPropertyName("user_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "user_id field is required")]
        public string UserId { get; set; }
    }
}
