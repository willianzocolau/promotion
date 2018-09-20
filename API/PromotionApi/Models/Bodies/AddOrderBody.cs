using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class AddOrderBody
    {
        [JsonProperty("promotion_id")]
        public long PromotionId { get; set; }
        [JsonProperty("user_id")]
        public long UserId { get; set; }
    }
}
