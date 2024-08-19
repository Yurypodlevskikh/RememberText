using RememberText.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.ViewModels
{
    public class PreferredLangViewModel
    {
        public string SourceLang { get; set; }
        public List<Language> PrefLangs { get; set; }
    }
}
