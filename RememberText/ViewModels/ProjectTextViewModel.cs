using System.Collections.Generic;

namespace RememberText.ViewModels
{
    public class ProjectTextViewModel
    {
        public int TopicId { get; set; }
        public string TopicTitle { get; set; }
        public string CopyrightName { get; set; }
        public string SourceLang { get; set; }
        public string TargetLang { get; set; }
        public string HowToDisplay { get; set; }
        public string ReturnUrl { get; set; }
        public List<SourceTargetSentence> SourceText { get; set; }
        public List<SourceTargetSentence> TargetText { get; set; }
    }

    public class SourceTargetSentence
    {
        public int SentenceIndex { get; set; }
        public string Sentence { get; set; }
        public bool Breaking { get; set; }
    }

    public class EditProjectViewModel
    {
        public int TopicId { get; set; }
        public string TopicTitle { get; set; }
        public int? SourceLangId { get; set; }
        public int TargetLangId { get; set; }
        public string SourceLang { get; set; }
        public string TargetLang { get; set; }
        public string WhatToEdit { get; set; }
        public string ReturnUrl { get; set; }
        public int? CopyrightId { get; set; }
        public string CopyrightName { get; set; }
        public int SourceTextId { get; set; }
        public List<SourceTargetSentence> SourceText { get; set; }
        public int? TargetTextId { get; set; }
        public List<SourceTargetSentence> TargetText { get; set; }
    }
}
