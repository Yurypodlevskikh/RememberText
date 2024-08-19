using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class AllAvailableLangViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRTLanguageService _languageService;

        public AllAvailableLangViewComponent(UserManager<User> userManager, SignInManager<User> signInManager, IRTLanguageService languageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _languageService = languageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string selectedLang) => View(await GetLanguages(selectedLang));

        private async Task<List<AllAvailableLangViewModel>> GetLanguages(string selectedLang)
        {
            var languages = await _languageService.GetAllAvailableLangAsync();
            var model = LanguageMapping.SortLangByPimaryLang(languages.ToList(), selectedLang);

            return model.ToList();
        }
    }
}
