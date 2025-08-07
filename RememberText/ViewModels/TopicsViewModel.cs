using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.ViewModels
{
    public class TopicsViewModel
    {
        public int TopicId { get; set; }
        [Display(Name = "Title")]
        public string TopicTitle { get; set; }
        [Display(Name = "Source")]
        public string SourceLang { get; set; }
        [Display(Name = "Target")]
        public string TargetLang { get; set; }
        [Display(Name = "Created")]
        public DateTime CreatedDateTime { get; set; }
        [Display(Name = "Updated")]
        public DateTime? UpdatedDateTime { get; set; }
        public bool? PublicText { get; set; }
        [Display(Name = "Nickname")]
        public string AuthorNickname { get; set; }
        public bool BanProject { get; set; }
    }

    public class TopicsWithLangFlagsViewModel : TopicsViewModel
    {
        public string SourceBasicFlag { get; set; }
        public string TargetBasicFlag { get; set; }
        public string SourceLangName { get; set; }
        public string TargetLangName { get; set; }
        public string SourceFlag { get; set; }
        public string TargetFlag { get; set; }
    }
}
