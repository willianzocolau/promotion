using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class LoginResponse
    {
        [JsonProperty("token"), Required]
        public string Token { get; set; }

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

        [JsonProperty("credit"), Required]
        public double Credit { get; set; }

        [JsonProperty("email"), Required]
        public string Email { get; set; }

        [JsonProperty("name"), Required]
        public string Name { get; set; }

        [JsonProperty("state")]
        public long? StateFK { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("cellphone")]
        public string Cellphone { get; set; }
    }
}
