using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class State
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(45)]
        public string Name { get; set; }
    }
}
