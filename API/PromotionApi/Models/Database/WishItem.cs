using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class WishItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(45)]
        public string Name { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
        [ForeignKey("UserFK")]
        public User User { get; set; }
        public long UserFK { get; set; }
    }
}
