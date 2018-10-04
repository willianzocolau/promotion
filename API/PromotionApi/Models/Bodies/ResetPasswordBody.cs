using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class ResetPasswordBody
    {
        [JsonProperty("email"), Required]
        public string Email { get; set; }
    }
}
