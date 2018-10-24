using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class AnswerBody
    {
        [JsonProperty("answer"), Required]
        public string Answer { get; set; }
    }
}
