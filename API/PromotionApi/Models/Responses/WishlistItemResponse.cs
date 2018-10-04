using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class WishlistItemResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("name"), Required]
        public string Name { get; set; }

        [JsonProperty("register_date"), Required]
        public DateTimeOffset RegisterDate { get; set; }
    }
}
