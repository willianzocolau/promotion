using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class VoteResponse
    {
        [JsonProperty("is_positive"), Required]
        public bool IsPositive { get; set; }

        [JsonProperty("comment"), Required]
        public string Comment { get; set; }

        [JsonProperty("comment_register_date"), Required]
        public DateTimeOffset CommentRegisterDate { get; set; }

        [JsonProperty("answer"), Required]
        public string Answer { get; set; }

        [JsonProperty("answer_register_date"), Required]
        public DateTimeOffset? AnswerRegisterDate { get; set; }

        [JsonProperty("order_id"), Required]
        public long OrderId { get; set; }

        [JsonProperty("user"), Required]
        public UserResponse User { get; set; }
    }
}
