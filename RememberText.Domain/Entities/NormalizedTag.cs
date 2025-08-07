using RememberText.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities
{
    public class NormalizedTag : RowVersionEntity
    {
        [Required]
        [StringLength(256)]
        public string NormalizedTagName { get; set; }
    }
}
