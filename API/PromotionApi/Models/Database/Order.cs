using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
        [ForeignKey("ApprovedByUserFK")]
        public User ApprovedByUser { get; set; }
        public long? ApprovedByUserFK { get; set; }
        [ForeignKey("UserFK")]
        public User User { get; set; }
        public long UserFK { get; set; }
        [ForeignKey("PromotionFK")]
        public Promotion Promotion { get; set; }
        public long PromotionFK { get; set; }
    }
}
