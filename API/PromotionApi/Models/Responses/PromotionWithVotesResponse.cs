using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class PromotionWithVotesResponse : PromotionResponse
    {
        [JsonProperty("votes"), Required]
        public HashSet<VoteResponse> Votes { get; set; }
    }
}
