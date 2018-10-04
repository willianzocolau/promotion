using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class RegisterResponse
    {
        [JsonProperty("token"), Required]
        public string Token { get; set; }
    }
}
