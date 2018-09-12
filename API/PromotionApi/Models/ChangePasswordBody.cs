namespace PromotionApi.Models
{
    public class ChangePasswordBody
    {
        public string ResetCode { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
