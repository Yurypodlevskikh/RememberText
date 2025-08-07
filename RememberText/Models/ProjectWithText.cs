using RememberText.Domain.Entities;
using RememberText.ViewModels;
using System.Collections.Generic;

namespace RememberText.Models
{
    public class ProjectWithText
    {
        public int TopicId { get; set; }
        public string TopicTitle { get; set; }
        public int SourceLangId { get; set; }
        public string SourceLang { get; set; }
        public int TargetLangId { get; set; }
        public string TargetLang { get; set; }
        public string ProjectAuthorId { get; set; }
        public string Author { get; set; }
        public string HowToDisplay { get; set; }
        public string ReturnUrl { get; set; }
        public int SourceTextId { get; set; }
        public TextCopyrightModel Copyright { get; set; }
        public List<SourceTargetSentence> SourceText { get; set; }
        public int? TargetTextId { get; set; }
        public List<SourceTargetSentence> TargetText { get; set; }
    }
}
