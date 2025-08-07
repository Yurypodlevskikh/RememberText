using System.ComponentModel.DataAnnotations;

namespace RememberText.Models
{
    public class EditTextCopyright
    {
        [Required]
        public int TopicId { get; set; }
        public int? CopyrightId { get; set; }
        [Required]
        [StringLength(150)]
        public string CopyrightName { get; set; }
    }
}
