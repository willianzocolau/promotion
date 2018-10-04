using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class UserSearchResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("nickname"), Required]
        public string Nickname { get; set; }
    }
}
