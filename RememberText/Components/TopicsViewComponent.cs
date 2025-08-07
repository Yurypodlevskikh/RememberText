using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class TopicsViewComponent : ViewComponent
    {
        private readonly IRTTopicService _topicService;
        private readonly UserManager<User> _userManager;
        private readonly IRTLanguageService _languageService;

        public TopicsViewComponent(IRTTopicService topicService, 
            UserManager<User> userManager,
            IRTLanguageService languageService)
        {
            _topicService = topicService;
            _userManager = userManager;
            _languageService = languageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string shownFor, string sortParam, int? pageNumber, string tagParam, string searchTitle)
        {
            shownFor = string.IsNullOrEmpty(shownFor) ? "PublicTopics" : shownFor; 
            string viewName = shownFor;

            if (shownFor == "PublicTopics" || shownFor == "TagPublicTopics")
            {
                viewName = "PublicTopics";
            }
            else if(shownFor == "UserTopics" || shownFor == "TagTopics")
            {
                viewName = "UserTopics";
            }

            return View(viewName, await GetTopics(shownFor, sortParam, pageNumber, tagParam, searchTitle));
        }
            
        private async Task<TopicsAndParamViewModel> GetTopics(string shownFor, string sortParam, int? pageNumber, string tagParam, string searchTitle)
        {
            IEnumerable<TopicsWithLangFlagsViewModel> topicsToView = null;
            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

            if(shownFor == "PublicTopics" || shownFor == "TagPublicTopics")
            {
                if(shownFor == "TagPublicTopics")
                {
                    if(!string.IsNullOrEmpty(tagParam))
                    {
                        string[] tagParamArr = tagParam.Split('_');
                        if(tagParamArr.Length == 2)
                        {
                            string langCode = tagParamArr[0];
                            if(Int32.TryParse(tagParamArr[1], out int tagId))
                            {
                                if(user != null)
                                {
                                    // Get published Topics except User's topics
                                    List<TopicsViewModel> topicCollect = await _topicService.GetPublishedTopicsByTagIdExceptUsersTopics(tagId, langCode, user.Id);
                                    topicsToView = await AddFlagsToTopics(topicCollect);
                                }
                                else
                                {
                                    // Get published Topics for anonimous Users
                                    List<TopicsViewModel> topicCollect = await _topicService.GetPublishedTopicsByTagIdAndLangCode(tagId, langCode);
                                    topicsToView = await AddFlagsToTopics(topicCollect);
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<TopicsViewModel> topicCollect = await _topicService.GetAllPublishedTopics();
                    topicsToView = await AddFlagsToTopics(topicCollect);
                }
            }
            else
            {
                if(user != null)
                {
                    if (shownFor == "TagTopics")
                    {
                        // Get User's topics by tag
                        if (!string.IsNullOrEmpty(tagParam))
                        {
                            string[] tagParamArr = tagParam.Split('_');
                            if (tagParamArr.Length == 2)
                            {
                                string langCode = tagParamArr[0];
                                if (Int32.TryParse(tagParamArr[1], out int tagId))
                                {
                                    List<TopicsViewModel> topicCollect = await _topicService.GetUsersTopicsByTagId(tagId, langCode, user.Id);
                                    topicsToView = await AddFlagsToTopics(topicCollect);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Get User's topics
                        List<TopicsViewModel> topicCollect = await _topicService.GetAllTopicsByLoggedInUser(user.Id);
                        topicsToView = await AddFlagsToTopics(topicCollect);
                    }
                }
            }

            // Topics searching
            if(!string.IsNullOrEmpty(searchTitle))
            {
                topicsToView = topicsToView.Where(x => x.TopicTitle.Contains(searchTitle));
            }

            // Topics sorting
            topicsToView = topicsToView.SortTopics(sortParam);

            int pageSize = 10;

            sortParam = string.IsNullOrEmpty(sortParam) ? "title" : sortParam;

            TopicsSortParamViewModel topicsSortParam = new TopicsSortParamViewModel
            {
                TitleSortParam = sortParam == "title" ? "title_desc" : "title",
                PublicSortParam = sortParam == "public" ? "public_desc" : "public",
                SourceSortParam = sortParam == "source" ? "source_desc" : "source",
                TargetSortParam = sortParam == "target" ? "target_desc" : "target",
                CreatedSortParam = sortParam == "created" ? "created_desc" : "created",
                UpdatedSortParam = sortParam == "updated" ? "updated_desc" : "updated"
            };

            int totalPages = (int)Math.Ceiling(topicsToView.Count() / (double)pageSize);
            int page = pageNumber != null ? pageNumber > totalPages ? totalPages : (int)pageNumber : 1;

            PaginatedTopics pt = new PaginatedTopics
            {
                PLTopics = PaginatedList<TopicsWithLangFlagsViewModel>.Create(topicsToView, page, pageSize),
                ViewMode = shownFor,
                SortOrder = sortParam,
                PageNumber = page,
                TagParam = tagParam,
                SearchTitle = searchTitle
            };

            TopicsAndParamViewModel topicsNSortParam = new TopicsAndParamViewModel
            {
                TopicSortParam = topicsSortParam,
                Topics = pt
            };

            return topicsNSortParam;
        }

        private async Task<List<TopicsWithLangFlagsViewModel>> AddFlagsToTopics(List<TopicsViewModel> topicCollect)
        {
            var topicsToView = new List<TopicsWithLangFlagsViewModel>();

            if (topicCollect != null && topicCollect.Count > 0)
            {
                foreach (var topic in topicCollect)
                {
                    var topicToView = new TopicsWithLangFlagsViewModel
                    {
                        TopicId = topic.TopicId,
                        TopicTitle = topic.TopicTitle,
                        SourceLang = topic.SourceLang,
                        TargetLang = topic.TargetLang,
                        CreatedDateTime = topic.CreatedDateTime,
                        UpdatedDateTime = topic.UpdatedDateTime,
                        PublicText = topic.PublicText,
                        AuthorNickname = topic.AuthorNickname,
                        BanProject = topic.BanProject
                    };

                    Language sourceLang = await _languageService.GetLanguageByLangCodeAsync(topic.SourceLang);
                    if (sourceLang != null)
                    {
                        topicToView.SourceLangName = sourceLang.LangName;
                        topicToView.SourceFlag = sourceLang.LangCode.Split('-').Last();
                        if (sourceLang.PrimaryLang == false)
                        {
                            Language sourceCommonLang = await _languageService.GetPrimaryLangByCommonLang(sourceLang.LangCode.Split('-').First());
                            topicToView.SourceBasicFlag = sourceCommonLang?.LangCode.Split('-').Last();
                        }
                    }

                    if (!string.IsNullOrEmpty(topic.TargetLang))
                    {
                        Language targetLang = await _languageService.GetLanguageByLangCodeAsync(topic.TargetLang);
                        if (targetLang != null)
                        {
                            topicToView.TargetLangName = targetLang.LangName;
                            topicToView.TargetFlag = targetLang.LangCode.Split('-').Last();
                            if (targetLang.PrimaryLang == false)
                            {
                                Language targetCommonLang = await _languageService.GetPrimaryLangByCommonLang(targetLang.LangCode.Split('-').First());
                                topicToView.SourceBasicFlag = targetCommonLang?.LangCode.Split('-').Last();
                            }
                        }
                    }

                    topicsToView.Add(topicToView);
                }
            }

            return topicsToView;
        }
    }
}
