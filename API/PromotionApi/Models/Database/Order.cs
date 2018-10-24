using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionApi.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTimeOffset RegisterDate { get; set; }

        #region Vote
        public bool? IsVotePositive { get; set; }

        [MaxLength(400)]
        public string Comment { get; set; }

        [MaxLength(400)]
        public string Answer { get; set; }

        public DateTimeOffset? CommentRegisterDate { get; set; }

        public DateTimeOffset? AnswerRegisterDate { get; set; }
        #endregion

        [ForeignKey("ApprovedByUserFK")]
        public virtual User ApprovedByUser { get; set; }
        public long? ApprovedByUserFK { get; set; }

        [ForeignKey("UserFK")]
        public virtual User User { get; set; }
        public long UserFK { get; set; }

        [ForeignKey("PromotionFK")]
        public virtual Promotion Promotion { get; set; }
        public long PromotionFK { get; set; }
    }
}
