using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class ChangePasswordBody
    {
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
        [JsonProperty("reset_code")]
        public string ResetCode { get; set; }
        [JsonProperty("old_password")]
        public string OldPassword { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
