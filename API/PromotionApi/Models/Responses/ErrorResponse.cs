using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class ErrorResponse
    {
        [JsonProperty("error"), Required]
        public string Error { get; set; }
    }
}
