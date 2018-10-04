using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class AddOrderBody
    {
        [JsonProperty("promotion_id"), Required]
        public long PromotionId { get; set; }

        [JsonProperty("user_id"), Required]
        public long UserId { get; set; }
    }
}
