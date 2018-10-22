using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class MatchItemResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("register_date"), Required]
        public DateTimeOffset RegisterDate { get; set; }

        [JsonProperty("is_active"), Required]
        public bool IsActive { get; set; }

        [JsonProperty("user_id"), Required]
        public long UserId { get; set; }

        [JsonProperty("promotion_id"), Required]
        public long PromotionId { get; set; }
    }
}
