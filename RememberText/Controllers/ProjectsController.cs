using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RememberText.Infrastructure.Interfaces;
using RememberText.RTTools;
using RememberText.ViewModels;

namespace RememberText.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRTLanguageService _languageService;

        public ProjectsController(IHttpContextAccessor httpContextAccessor, IRTLanguageService languageService)
        {
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
        }

        public async Task<IActionResult> Index() 
        {
            return View(); 
        }

        public IActionResult Create()
        {
            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_TypeOfProject");
            }

            return View();
        }

        public IActionResult CreateSingleText()
        {
            if (HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_CreateSingleText");
            }

            return View();
        }

        public IActionResult DefineSourceLang()
        {
            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_DefineSourceLang");
            }

            return View();
        }

        public IActionResult DefineTargetLang(string langCode)
        {
            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_DefineTargetLang", langCode);
            }

            return View("DefineTargetLang", langCode);
        }

        public async Task<IActionResult> CreateTopic(string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
            {
                return RedirectToAction(nameof(Create));
            }

            TopicFieldViewModel topic = new TopicFieldViewModel();
            if (langCode.IndexOf("_") != -1)
            {
                // Get Language Options
                var langpar = langCode.Split('_').ToArray();
                if(langpar.Length == 2)
                {
                    if(await LangCodeExists(langpar[0]) && await LangCodeExists(langpar[1]))
                    {
                        topic.SourceLang = langpar[0];
                        topic.TargetLang = langpar[1];
                    }
                }
            }
            else
            {
                if (await LangCodeExists(langCode))
                {
                    topic.SourceLang = langCode;
                }
            }

            // Last validation
            if (string.IsNullOrEmpty(topic.SourceLang))
            {
                TempData["WrongResponse"] = "Wrong Language Code!";
                return RedirectToAction(nameof(Index));
            }

            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_CreateTopic", topic);
            }

            return View("CreateTopic", topic);
        }

        private async Task<bool> LangCodeExists(string langCode)
        {
            var lang = await _languageService.GetLanguageByLangCodeAsync(langCode);
            return lang != null;
        }

        [HttpPost]
        public IActionResult CreateTopic(TopicFieldViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return View();
        }
    }
}
