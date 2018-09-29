using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class AddWishItemBody
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
