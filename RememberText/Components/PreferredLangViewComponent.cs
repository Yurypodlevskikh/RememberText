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
        private readonly IRTTagService _tagService;
        private readonly IRTTopicService _topicService;

        public PreferredLangViewComponent(
            UserManager<User> userManager, 
            IRTLanguageService LanguageService, 
            IRTTagService tagService,
            IRTTopicService topicService) 
        {
            _userManager = userManager;
            _LanguageService = LanguageService;
            _tagService = tagService;
            _topicService = topicService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string whereLang)
        {
            string view = whereLang.IndexOf("-") != -1 ? "CreateTopic" : whereLang;
            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            var sidebarLangTags = new List<SidebarLangTagsViewModel>();

            if (whereLang == "SidebarLangTags")
            {
                if (user != null)
                {
                    // Get available tags from opened Projects to registered User
                    sidebarLangTags = await GetAvailableTagsByLang(user.PreffLang, user.Id, whereLang);
                }
                else
                {
                    // Get available tags from opened Projects if the User is not registered
                    sidebarLangTags = await GetAvailableTagsByLang();
                }

                //return View(whereLang, sidebarLangTags);

                return View(whereLang, BaseLangWithBaseTags(sidebarLangTags));
            }
            else if(whereLang == "SidebarUserLangTags")
            {
                if (user != null)
                {
                    // Get available User's tags from User's Projects
                    sidebarLangTags = await GetAvailableTagsByLang(user.PreffLang, user.Id, whereLang);
                    //return View(whereLang, sidebarLangTags);
                    return View(whereLang, BaseLangWithBaseTags(sidebarLangTags));
                }
            }
            else
            {
                if (user != null)
                {
                    return View(view, await GetSelectedLanguages(whereLang, user.PreffLang));
                }
            }
            
            return View("Default");
        }

        private async Task<List<SidebarLangTagsViewModel>> GetAvailableTagsByLang(string userLangStr = "", string userId = "", string viewName = "")
        {
            var tags = new List<SidebarLangTagsViewModel>();
            
            if (!string.IsNullOrEmpty(viewName))
            {
                if (!string.IsNullOrEmpty(userLangStr) && !string.IsNullOrEmpty(userId))
                {
                    var userLangs = await _LanguageService.GetUserPreferredLanguagesAsync(userLangStr);
                    List<TagAndTaggedTexts> tagsRelativeUser = null;

                    foreach (var lang in userLangs)
                    {
                        if (viewName == "SidebarLangTags")
                        {
                            // Get tags connected to the published projects by User's Languages except User's projects
                            tagsRelativeUser = await _tagService.GetPublishedTagsExceptUsersByUsersLangCode(lang.LangCode, userId);
                        }
                        else if (viewName == "SidebarUserLangTags")
                        {
                            // Get tags connected to the only User's published Projects by User's languages
                            tagsRelativeUser = await _tagService.GetUsersTagsByUsersLangCode(lang.LangCode, userId);
                        }

                        if (tagsRelativeUser != null && tagsRelativeUser.Count() > 0)
                        {
                            var langTags = new SidebarLangTagsViewModel
                            {
                                LangCode = lang.LangCode,
                                LangName = lang.LangName,
                                Tags = tagsRelativeUser.OrderByDescending(x => x.TaggedTopics).ThenBy(x => x.TagName)
                            };
                            tags.Add(langTags);
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(viewName))
            {
                // Get published tags
                if (string.IsNullOrEmpty(userLangStr) && string.IsNullOrEmpty(userId))
                {
                    // Get pablished Projects
                    IEnumerable<TopicsViewModel> topics = await _topicService.GetAllPublishedTopics();

                    if (topics != null && topics.Count() > 0)
                    {
                        // Collect languages
                        List<string> publicLang = new List<string>();
                        foreach (var topic in topics)
                        {
                            if (!string.IsNullOrEmpty(topic.SourceLang))
                            {
                                if (!publicLang.Contains(topic.SourceLang))
                                {
                                    publicLang.Add(topic.SourceLang);
                                }

                                if (!string.IsNullOrEmpty(topic.TargetLang))
                                {
                                    if (!publicLang.Contains(topic.TargetLang))
                                    {
                                        publicLang.Add(topic.TargetLang);
                                    }
                                }
                            }
                        }

                        // Get tags from the languages
                        foreach (var l in publicLang)
                        {
                            List<TagAndTaggedTexts> tgs = await _tagService.GetPublishedTagsByLangCode(l);

                            if (tgs != null && tgs.Count() > 0)
                            {
                                Language langData = await _LanguageService.GetLanguageByLangCodeAsync(l);
                                if (langData != null)
                                {
                                    var langTags = new SidebarLangTagsViewModel
                                    {
                                        LangCode = l,
                                        LangName = langData.LangName,
                                        Tags = tgs.OrderByDescending(x => x.TaggedTopics).ThenBy(x => x.TagName)
                                    };
                                    tags.Add(langTags);
                                }
                            }
                        }
                    }
                }
            }

            return tags.OrderBy(x => x.LangCode).ToList();
        }

        private async Task<PreferredLangViewModel> GetSelectedLanguages(string whereLang, string userLangStr)
        {
            PreferredLangViewModel model = new PreferredLangViewModel();

            // If whereLang is a language code.
            if (whereLang.IndexOf("-") != -1)
            {
                model.SourceLang = whereLang;
            }

            var pLang = await _LanguageService.GetUserPreferredLanguagesAsync(userLangStr);
            model.PrefLangs = pLang.OrderBy(x => x.LangName).ToList();

            return model;
        }

        /// <summary>
        /// Sorting: base language with related languages and their tags.
        /// </summary>
        /// <param name="sidebarLangTags">Collection from the database</param>
        /// <returns>Collection for menu: First collaps button with two common language characters and 
        /// after it characters of other related languages. Then list of tags to each language</returns>
        private List<SidebarTagListsByLangViewModel> BaseLangWithLangsAndTags(List<SidebarLangTagsViewModel> sidebarLangTags)
        {
            var sidebarLangTagLists = new List<SidebarTagListsByLangViewModel>();
            if (sidebarLangTags != null)
            {
                List<SidebarLangTagsViewModel> listTags = null;
                string baseLang = "";
                int langTagCount = sidebarLangTags.Count();
                int numberOfTags = 0;
                for (int i = 0; i < langTagCount; i++)
                {
                    string currBaseLang = sidebarLangTags[i].LangCode.Split('-')[0];
                    if (i == 0)
                    {
                        // Add first Item of first Language with tags
                        baseLang = currBaseLang;
                        listTags = new List<SidebarLangTagsViewModel>();
                        listTags.Add(sidebarLangTags[i]);
                        numberOfTags = sidebarLangTags[i].NumberOfTags;
                    }
                    else
                    {
                        if (baseLang == currBaseLang)
                        {
                            // If next Item has the same language,
                            // just add it to the same List of Language with tags
                            listTags.Add(sidebarLangTags[i]);
                            numberOfTags = numberOfTags += sidebarLangTags[i].NumberOfTags;
                        }
                        else
                        {
                            // If next Item has other language,
                            // sort created before List
                            var sidebarListTags = new List<SidebarLangTagsViewModel>();
                            sidebarListTags = listTags.Count() > 1 ? listTags.OrderBy(x => x.NumberOfTags).ToList() : listTags;
                            int numberOfAllTags = numberOfTags;

                            // and add this List to the Common List of all Languages with tags
                            var sidebarLangTagList = new SidebarTagListsByLangViewModel
                            {
                                LangTags = sidebarListTags,
                                NumberOfAllTags = numberOfAllTags
                            };
                            sidebarLangTagLists.Add(sidebarLangTagList);

                            // Create new List of Language with tags for the next Language
                            listTags = new List<SidebarLangTagsViewModel>();

                            baseLang = currBaseLang;
                            // Create a next List of Language with tags
                            listTags.Add(sidebarLangTags[i]);
                            numberOfTags = sidebarLangTags[i].NumberOfTags;
                        }
                    }

                    // If the loop has no more Items,
                    if (i == (langTagCount - 1))
                    {
                        var sidebarListTags = new List<SidebarLangTagsViewModel>();
                        sidebarListTags = listTags.Count() > 1 ? listTags.OrderBy(x => x.NumberOfTags).ToList() : listTags;
                        int numberOfAllTags = numberOfTags;

                        // then add this List to the Common List of all Languages with tags
                        var sidebarLangTagList = new SidebarTagListsByLangViewModel
                        {
                            LangTags = sidebarListTags,
                            NumberOfAllTags = numberOfAllTags
                        };

                        sidebarLangTagLists.Add(sidebarLangTagList);
                    }
                }
            }

            return sidebarLangTagLists;
        }

        private List<SidebarBaseLangTagsViewModel> BaseLangWithBaseTags(List<SidebarLangTagsViewModel> sidebarLangTags)
        {
            var sidebarBaseLangTags = new List<SidebarBaseLangTagsViewModel>();

            if(sidebarLangTags != null)
            {
                SidebarBaseLangTagsViewModel langAndTags = null;
                string baseLangCode = "";
                int langCount = sidebarLangTags.Count();
                
                for (int i = 0; i < langCount; i++)
                {
                    string currBaseLangCode = sidebarLangTags[i].LangCode.Split('-')[0];
                    if (i == 0)
                    {
                        // Add first Item of first Language with tags
                        baseLangCode = currBaseLangCode;

                        langAndTags = new SidebarBaseLangTagsViewModel();
                        langAndTags.BaseLangCode = currBaseLangCode;
                        langAndTags.BaseLangName = sidebarLangTags[i].LangName.Split('-')[0];
                        langAndTags.BaseTags = sidebarLangTags[i].Tags.ToList();
                        langAndTags.NumberOfAllTags = sidebarLangTags[i].NumberOfTags;
                    }
                    else
                    {
                        if (baseLangCode == currBaseLangCode)
                        {
                            // If next Item has the same language,
                            // just add these tags to the previous list of tags
                            foreach(var tag in sidebarLangTags[i].Tags)
                            {
                                var tagExists = langAndTags.BaseTags.FirstOrDefault(x => x.Id == tag.Id);
                                if (tagExists != null)
                                {
                                    tagExists.TaggedTopics += tag.TaggedTopics; 
                                }
                                else
                                {
                                    langAndTags.BaseTags.Add(tag);
                                    langAndTags.NumberOfAllTags += sidebarLangTags[i].NumberOfTags;
                                }
                            }
                        }
                        else
                        {
                            // If next Item has other language,
                            // sort created before List
                            if(langAndTags.BaseTags != null && langAndTags.BaseTags.Count() > 1)
                            {
                                langAndTags.BaseTags.OrderBy(x => x.TaggedTopics).ToList();
                            }
                            
                            //var sidebarListTags = new List<SidebarLangTagsViewModel>();
                            //sidebarListTags = langAndTags.Count() > 1 ? langAndTags.OrderBy(x => x.NumberOfTags).ToList() : langAndTags;
                            //int numberOfAllTags = numberOfTags;

                            // and add this List to the Common List of all Languages with tags
                            sidebarBaseLangTags.Add(langAndTags);

                            // Create new List of Language with tags for the next Language
                            langAndTags = new SidebarBaseLangTagsViewModel();

                            baseLangCode = currBaseLangCode;
                            // Create a next List of Language with tags
                            langAndTags = new SidebarBaseLangTagsViewModel();
                            langAndTags.BaseLangCode = currBaseLangCode;
                            langAndTags.BaseLangName = sidebarLangTags[i].LangName.Split('-')[0];
                            langAndTags.BaseTags = sidebarLangTags[i].Tags.ToList();
                            langAndTags.NumberOfAllTags = sidebarLangTags[i].NumberOfTags;
                        }
                    }

                    // If the loop has no more Items,
                    if (i == (langCount - 1))
                    {
                        if (langAndTags.BaseTags != null && langAndTags.BaseTags.Count() > 1)
                        {
                            langAndTags.BaseTags.OrderBy(x => x.TaggedTopics).ToList();
                        }

                        // then add this List to the Common List of all Languages with tags
                        sidebarBaseLangTags.Add(langAndTags);
                    }
                }
            }

            return sidebarBaseLangTags;
        }
    }
}
