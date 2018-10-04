using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class StateResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("name"), Required]
        public string Name { get; set; }
    }
}
