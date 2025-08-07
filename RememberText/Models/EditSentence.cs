using RememberText.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Models
{
    public class EditSentence
    {
        [Required]
        public int TopicId { get; set; }
        public string WhatToEdit { get; set; }
        [Required]
        public int TargetTextId { get; set; }
        [Required]
        public string TargetLang { get; set; }
        [Required]
        public int TargetTextIndex { get; set; }
        public string Sentence { get; set; }
        [Required]
        public string ActionBtn { get; set; }
    }

    public class JsonResponceToAlertBox
    {
        public string AlertClasses { get; set; }
        public string ResponseText { get; set; }
    }

    public class UpdateSentenceFormText
    {
        public int SentenceIndex { get; set; }
        public int SourceTextId { get; set; }
        public string SourceLang { get; set; }
        public List<SourceTargetSentence> SourceText { get; set; }
        public int? TargetTextId { get; set; }
        public string TargetLang { get; set; }
        public List<SourceTargetSentence> TargetText { get; set; }
    }

    public class AddLinesToText
    {
        [Required]
        public int TopicId { get; set; }
        public string WhatToEdit { get; set; }
        [Required]
        public int NumberOfLines { get; set; }
    }
}
