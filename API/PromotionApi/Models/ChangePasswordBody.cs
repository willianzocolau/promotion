namespace PromotionApi.Models
{
    public class ChangePasswordBody
    {
        public string NewPassword { get; set; }
        public string ResetCode { get; set; }
        public string OldPassword { get; set; }
        public string Email { get; set; }
    }
}
