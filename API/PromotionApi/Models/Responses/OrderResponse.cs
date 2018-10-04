using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class OrderResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("register_date"), Required]
        public DateTimeOffset RegisterDate { get; set; }

        [JsonProperty("approved_by"), Required]
        public long? ApprovedByUserFK { get; set; }

        [JsonProperty("user_id"), Required]
        public long UserFK { get; set; }

        [JsonProperty("promotion_id"), Required]
        public long PromotionFK { get; set; }
    }
}
