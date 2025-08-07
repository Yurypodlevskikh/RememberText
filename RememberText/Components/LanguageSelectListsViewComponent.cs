using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RememberText.Infrastructure.Interfaces;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class LanguageSelectListsViewComponent : ViewComponent
    {
        private readonly IRTLanguageService _LanguageService;
        private readonly IOptions<RequestLocalizationOptions> _locOptions;

        public LanguageSelectListsViewComponent(IRTLanguageService LanguageService, IOptions<RequestLocalizationOptions> locOptions)
        {
            _LanguageService = LanguageService;
            _locOptions = locOptions;
        }

        public async Task<IViewComponentResult> InvokeAsync(string fieldName)
        {
            switch (fieldName)
            {
                case "PreferredLang":
                    return View(fieldName, await GetLanguages(fieldName));
                case "SelectLanguage":
                    return View(fieldName, GetLanguageSelectList());
            }

            return null;
        }

        private async Task<ChckBxsViewModel> GetLanguages(string fieldName)
        {
            IEnumerable<Domain.Entities.Language> primLang = await _LanguageService.PrimaryLanguagesAsync();
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

        private SelectList GetLanguageSelectList()
        {
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var cultureItems = _locOptions.Value.SupportedUICultures;

            return new SelectList(cultureItems, "Name", "NativeName", requestCulture.RequestCulture.UICulture.Name);
        }
    }
}
