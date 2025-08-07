using RememberText.Data;
using RememberText.Domain.Entities;
using RememberText.Models;
using System.Collections.Generic;
using System.Linq;

namespace RememberText.ViewModels
{
    public class PreferredLangViewModel
    {
        public string SourceLang { get; set; }
        public List<Language> PrefLangs { get; set; }
    }

    public class SidebarLangTagsViewModel
    {
        public string LangCode { get; set; }
        public string LangName { get; set; }
        public IEnumerable<TagAndTaggedTexts> Tags { get; set; }
        public int NumberOfTags => Tags != null ? Tags.Count() : 0;
    }

    public class SidebarTagListsByLangViewModel
    {
        public List<SidebarLangTagsViewModel> LangTags { get; set; }
        public int NumberOfAllTags { get; set; }

        public List<TagSidebarColor> SidebarColors = UsedColors.TagSidebarColors;
    }

    public class SidebarBaseLangTagsViewModel
    {
        public string BaseLangCode { get; set; }
        public string BaseLangName { get; set; }
        public List<TagAndTaggedTexts> BaseTags { get; set; }
        public int NumberOfAllTags { get; set; }
    }
}
