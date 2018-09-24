using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class Store
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(45)]
        public string Name { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
        [MaxLength(64)]
        public string Token { get; set; }
    }
}