using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class ForgotPasswordRequest
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(45)]
        public string Ip { get; set; }
        [MaxLength(6)]
        public string Code { get; set; }
        public DateTimeOffset RequestDate { get; set; }
        [ForeignKey("UserFK")]
        public virtual User User { get; set; }
        public long UserFK { get; set; }
    }
}
