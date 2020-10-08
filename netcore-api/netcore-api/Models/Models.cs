using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace netcore_api.Models
{
    [Serializable]
    public class KafkaMessageModel
    {
        [JsonPropertyName("message")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "message field is required")]
        public string Message { get; set; }
    }
}
