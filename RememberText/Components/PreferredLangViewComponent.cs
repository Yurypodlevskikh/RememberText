using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Interfaces;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class PreferredLangViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly IRTLanguageService _LanguageService;

        public PreferredLangViewComponent(UserManager<User> userManager, IRTLanguageService LanguageService) 
        {
            _userManager = userManager;
            _LanguageService = LanguageService; 
        }

        public async Task<IViewComponentResult> InvokeAsync(string whereLang)
        {
            string view = whereLang.IndexOf("-") != -1 ? "CreateTopic" : whereLang;
            
            return View(view, await GetSelectedLanguages(whereLang));
        }

        private async Task<PreferredLangViewModel> GetSelectedLanguages(string whereLang)
        {
            PreferredLangViewModel model = new PreferredLangViewModel();

            // If whereLang is a language code.
            if (whereLang.IndexOf("-") != -1)
            {
                model.SourceLang = whereLang;
            }

            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

            List<Language> selectedLang = new List<Language>();
            string[] selectedLangArr = user.PreffLang.Split(',');
            foreach(string langId in selectedLangArr)
            {
                if(!string.IsNullOrEmpty(langId))
                {
                    if(Int32.TryParse(langId, out int id))
                    {
                        var lang = await _LanguageService.GetLanguageByIdAsync(id);
                        if(lang != null)
                        {
                            selectedLang.Add(lang);
                        }    
                    }
                }
            }

            model.PrefLangs = selectedLang.OrderBy(x => x.LangCode).ToList();

            return model;
        }
    }
}
