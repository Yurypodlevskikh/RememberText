using System.ComponentModel.DataAnnotations;

namespace RememberText.ViewModels
{
    public class TopicFieldViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(256)]
        public string TopicTitle { get; set; }
        [Required]
        [StringLength(50)]
        public string SourceLang { get; set; }
        [StringLength(50)]
        public string TargetLang { get; set; }
        public int NumberOfLines { get; set; }
        public int AgeLimitation { get; set; }
        public int? CopyrightId { get; set; }
        public string CopyrightName { get; set; } 
    }
}
