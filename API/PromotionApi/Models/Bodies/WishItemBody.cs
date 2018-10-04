using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class WishlistItemBody
    {
        [JsonProperty("name"), Required]
        public string Name { get; set; }
    }
}
