using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.ViewModels
{
    public class TopicFieldViewModel
    {
        [Required]
        [StringLength(256)]
        public string TopicTitle { get; set; }
        [Required]
        [StringLength(50)]
        public string SourceLang { get; set; }
        [StringLength(50)]
        public string TargetLang { get; set; }
        public int NumberOfLines { get; set; }
    }
}
