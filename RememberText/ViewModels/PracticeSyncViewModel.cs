using System.Collections.Generic;

namespace RememberText.ViewModels
{
    public class PracticeSyncViewModel : ProjectTextViewModel
    {
        public int? SentenceIndex { get; set; }
        public string Answer { get; set; }
        public string CorrectAnswerIndexes { get; set; }
        public HashSet<string> CorrectAnswersIndexesHashSet{ get; set; }
        public string ResponseText { get; set; }
        public int SourceTextId { get; set; }
        public int? TargetTextId { get; set; }
    }
}
