using RememberText.Domain.Entities.Base.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities.Base
{
    public abstract class TagAssignmentEntity : ITagAssignmentEntity
    {
        [Required]
        public int TextId { get; set; }

        [Required]
        public int TagId { get; set; }
    }
}
