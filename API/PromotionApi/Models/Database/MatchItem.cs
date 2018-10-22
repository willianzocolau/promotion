using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class MatchItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTimeOffset RegisterDate { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("UserFK")]
        public virtual User User { get; set; }
        public long UserFK { get; set; }

        [ForeignKey("PromotionFK")]
        public virtual Promotion Promotion { get; set; }
        public long PromotionFK { get; set; }
    }
}
