using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(45)]
        public string Nickname { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(64)]
        public string Password { get; set; }
        public int Type { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
        [MaxLength(11)]
        public string Cpf { get; set; }
        public double Credit { get; set; }
        [MaxLength(150)]
        public string ImageUrl { get; set; }
        [MaxLength(11)]
        public string Telephone { get; set; }
        [MaxLength(11)]
        public string Cellphone { get; set; }
        [MaxLength(64)]
        public string Token { get; set; }
        [ForeignKey("StateFK")]
        public State State { get; set; }
        public int StateFK { get; set; }
    }
}
