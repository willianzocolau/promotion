using Newtonsoft.Json;

namespace PromotionApi.Models
{
    public class RegisterUserBody
    {
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("cpf")]
        public string Cpf { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}
