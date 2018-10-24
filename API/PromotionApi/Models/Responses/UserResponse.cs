using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class UserResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("nickname"), Required]
        public string Nickname { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("register_date"), Required]
        public DateTimeOffset RegisterDate { get; set; }

        [JsonProperty("type"), Required]
        public UserType Type { get; set; }

        [JsonProperty("seller_profile")]
        public SellerProfile SellerProfile { get; set; }
    }
}
