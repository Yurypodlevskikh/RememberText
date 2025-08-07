using RememberText.Domain.Entities.Base;
using RememberText.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RememberText.Domain.Entities
{
    public class GuestBookEntry : CreatedDateEntity
    {
        [StringLength(256)]
        public string MessageTitle { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get;set; }
    }
}
