namespace PromotionApi.Models
{
    public class RegisterPromotion
    {
        public string Name { get; set; }
        public string Price { get; set; }
        //public DateTimeOffset ExpireDate { get; set; }
        public string ImageUrl { get; set; }
    }
}