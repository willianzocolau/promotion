using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PromotionApi.Models
{
    public class SellerProfile
    {
        [JsonProperty("total_promotions"), Required]
        public int TotalPromotions { get; set; }

        [JsonProperty("total_orders"), Required]
        public int TotalOrders { get; set; }

        [JsonProperty("orders_upvotes"), Required]
        public int OrdersUpvotes { get; set; }

        [JsonProperty("order_downvotes"), Required]
        public int OrdersDownvotes { get; set; }
    }
}
