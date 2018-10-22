using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class MatchsResponse
    {
        [JsonProperty("total_active_matchs"), Required]
        public long TotalActiveMatchs { get; set; }

        [JsonProperty("matchs"), Required]
        public HashSet<MatchItemResponse> Matchs { get; set; }
    }
}
