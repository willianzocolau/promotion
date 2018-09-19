using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class RegisterPromotionBody
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
        //TODO: Add expire_date
        //[JsonProperty("expire_date")]
        //public DateTimeOffset ExpireDate { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("state_id")]
        public long StateFK { get; set; }
        [JsonProperty("store_id")]
        public long StoreFK { get; set; }
    }
}