using System;
using System.Collections.Generic;
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

        public bool Active { get; set; }

        public double? CashbackPercentage {get; set; }

        public DateTimeOffset RegisterDate { get; set; }

        public DateTimeOffset ExpireDate { get; set; }

        [MaxLength(150)]
        public string ImageUrl { get; set; }

        [ForeignKey("UserFK")]
        public virtual User User { get; set; }
        public long UserFK { get; set; }

        [ForeignKey("StateFK")]
        public virtual State State { get; set; }
        public long StateFK { get; set; }

        [ForeignKey("StoreFK")]
        public virtual Store Store { get; set; }
        public long StoreFK { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public Promotion()
        {
            Orders = new List<Order>();
        }
    }
}