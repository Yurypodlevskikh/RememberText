using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RememberText.Infrastructure.Interfaces;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class PrimaryLangChckBxViewComponent : ViewComponent
    {
        private readonly IRTLanguageService _LanguageService;

        public PrimaryLangChckBxViewComponent(IRTLanguageService LanguageService) => _LanguageService = LanguageService;

        public async Task<IViewComponentResult> InvokeAsync(string fieldName) => View(fieldName, await GetLanguages(fieldName));

        private async Task<ChckBxsViewModel> GetLanguages(string fieldName)
        {
            var primLang = await _LanguageService.PrimaryLanguagesAsync();
            List<SelectListItem> langChckBxs = new List<SelectListItem>();
            if (primLang != null)
            {
                foreach (var lang in primLang)
                {
                    SelectListItem langChckBx = new SelectListItem
                    {
                        Value = lang.Id.ToString(),
                        Text = lang.LangName.Split('-')[0].Trim(),
                        Selected = false
                    };
                    langChckBxs.Add(langChckBx);
                }
            }

            ChckBxsViewModel model = new ChckBxsViewModel
            {
                FieldsName = fieldName,
                Checks = langChckBxs
            };

            return model;
        }
    }
}
