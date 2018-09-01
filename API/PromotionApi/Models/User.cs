using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
        public DateTimeOffset RegisterDate { get; set; }
        public string Cpf { get; set; }
        public double Credit { get; set; }
        public string ImageUrl { get; set; }
        public string Telephone { get; set; }
        public string Cellphone { get; set; }
        public string Token { get; set; }
        [ForeignKey("StateFK")]
        public State State { get; set; }
        public long StateFK { get; set; }
    }
}
