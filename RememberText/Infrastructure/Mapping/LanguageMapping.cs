using RememberText.Domain.Entities;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Mapping
{
    public static class LanguageMapping
    {
        public static AllAvailableLangViewModel ToView(Language lang, int order, bool selected) => new AllAvailableLangViewModel
        {
            Id = lang.Id,
            LangCode = lang.LangCode,
            LangName = lang.LangName,
            Order = order,
            Selected = selected
        };

        public static IEnumerable<AllAvailableLangViewModel> SortLangByPimaryLang(List<Language> l, string selectedLang = "")
        {
            HashSet<int> selectedLangHSet = new HashSet<int>();
            if(!string.IsNullOrEmpty(selectedLang))
            {
                string[] selectedLangArr = selectedLang.Split(',');
                foreach(string sl in selectedLangArr)
                {
                    if(!string.IsNullOrEmpty(sl))
                    {
                        if(Int32.TryParse(sl, out int id))
                        {
                            selectedLangHSet.Add(id);
                        }
                    }
                }
            }

            List<AllAvailableLangViewModel> allAvailableLang = new List<AllAvailableLangViewModel>();

            string firstSyllable = "";
            int order = 1;
            int primaryIndex = 0;
            for (int i = 0; i < l.Count(); i++)
            {
                Language lang = l[i];
                bool selected = selectedLangHSet.Contains(lang.Id);

                if (i > 0)
                {
                    string currFirstSyllable = lang.LangCode.Split('-')[0];
                    if (firstSyllable == currFirstSyllable)
                    {
                        if ((bool)lang.PrimaryLang)
                        {
                            allAvailableLang.Insert(primaryIndex, LanguageMapping.ToView(lang, order, selected));
                        }
                        else
                        {
                            allAvailableLang.Add(LanguageMapping.ToView(lang, order, selected));
                        }
                    }
                    else
                    {
                        primaryIndex = i;
                        firstSyllable = currFirstSyllable;
                        order = ++order;

                        allAvailableLang.Add(LanguageMapping.ToView(lang, order, selected));
                    }
                }
                else
                {
                    firstSyllable = lang.LangCode.Split('-')[0];

                    allAvailableLang.Add(LanguageMapping.ToView(lang, order, selected));
                }
            }

            return allAvailableLang;
        }
    }
}
