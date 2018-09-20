using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class ResetPasswordBody
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
