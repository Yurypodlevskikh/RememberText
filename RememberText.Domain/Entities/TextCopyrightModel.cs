using RememberText.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities
{
    public class TextCopyrightModel : BaseEntity
    {
        [Required]
        [StringLength(150)]
        public string CopyrightName { get; set; }
        [Required]
        [StringLength(150)]
        public string NormalizedCopyrightName { get; set; }
    }
}
