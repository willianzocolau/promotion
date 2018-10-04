using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class TokenResponse
    {
        [JsonProperty("token"), Required]
        public string Token { get; set; }
    }
}
