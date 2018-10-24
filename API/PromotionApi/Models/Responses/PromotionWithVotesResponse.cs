using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class PromotionWithVotesResponse
    {
        [JsonProperty("id"), Required]
        public long Id { get; set; }

        [JsonProperty("name"), Required]
        public string Name { get; set; }

        [JsonProperty("price"), Required]
        public double Price { get; set; }

        [JsonProperty("register_date"), Required]
        public DateTimeOffset RegisterDate { get; set; }

        [JsonProperty("image_url"), Required]
        public string ImageUrl { get; set; }

        [JsonProperty("expire_date"), Required]
        public DateTimeOffset ExpireDate { get; set; }

        [JsonProperty("active"), Required]
        public bool Active { get; set; }

        [JsonProperty("user_id"), Required]
        public long UserFK { get; set; }

        [JsonProperty("state_id"), Required]
        public long StateFK { get; set; }

        [JsonProperty("store_id"), Required]
        public long StoreFK { get; set; }

        [JsonProperty("total_orders"), Required]
        public int TotalOrders { get; set; }

        [JsonProperty("order_upvotes"), Required]
        public int OrderUpvotes { get; set; }

        [JsonProperty("order_downvotes"), Required]
        public int OrderDownvotes { get; set; }

        [JsonProperty("votes"), Required]
        public HashSet<VoteResponse> Votes { get; set; }
    }
}
