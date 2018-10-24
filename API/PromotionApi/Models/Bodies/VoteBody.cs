using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class VoteBody
    {
        [JsonProperty("is_positive"), Required]
        public bool IsPositive { get; set; }

        [JsonProperty("comment"), Required]
        public string Comment { get; set; }
    }
}
