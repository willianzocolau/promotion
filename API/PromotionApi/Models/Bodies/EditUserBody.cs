using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class EditUserBody
    {
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("cpf")]
        public string Cpf { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("telephone")]
        public string Telephone { get; set; }
        [JsonProperty("cellphone")]
        public string Cellphone { get; set; }
    }
}
