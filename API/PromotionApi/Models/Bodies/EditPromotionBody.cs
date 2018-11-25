using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class EditPromotionBody
    {
        [JsonProperty("active")]
        public bool? Active { get; set; }
    }
}
