using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class EditWishItemBody
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
