using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class Promotion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(45)]
        public string Name { get; set; }
        public double Price {get; set; }
        public double? CashbackPercentage {get; set; }
        public DateTimeOffset RegisterDate { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
        [MaxLength(150)]
        public string ImageUrl { get; set; }
        [ForeignKey("UserFK")]
        public User User { get; set; }
        public long UserFK { get; set; }
        [ForeignKey("StateFK")]
        public State State { get; set; }
        public long StateFK { get; set; }
        [ForeignKey("StoreFK")]
        public Store Store { get; set; }
        public long StoreFK { get; set; }
    }
}