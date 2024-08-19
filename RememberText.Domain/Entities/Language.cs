using RememberText.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities
{
    public class Language : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string LangCode { get; set; }
        [Required]
        [StringLength(256)]
        public string LangName { get; set; }
        public bool? PrimaryLang { get; set; }
    }
}
