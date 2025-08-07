using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RememberText.DAL.Context;
using RememberText.Data;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.Models;
using RememberText.RTTools;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Controllers
{
    [Authorize]
    //[RequireHttps]
    [Route("Projects")]
    public class ProjectsController : Controller
    {
        private readonly RememberTextDbContext _db;
        private readonly IRTLanguageService _languageService;
        private readonly UserManager<User> _userManager;
        private readonly IRTTopicService _topicService;
        private readonly IRTTextService _textService;
        private readonly IStringLocalizer<ProjectsController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IRTTextCopyrightService _copyrightService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            RememberTextDbContext db,
            IRTLanguageService languageService, 
            UserManager<User> userManager,
            IRTTopicService topicService,
            IRTTextService textService,
            IStringLocalizer<ProjectsController> localizer,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IRTTextCopyrightService copyrightService,
            ILogger<ProjectsController> logger)
        {
            _db = db;
            _languageService = languageService;
            _userManager = userManager;
            _topicService = topicService;
            _textService = textService;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
            _copyrightService = copyrightService;
            _logger = logger;
        }

        [Route("Topics/{ViewMode:alpha?}/{TagParam?}")]
        [Route("Topics/{ViewMode:alpha?}/{SortOrder?}/{PageNumber:int?}/{TagParam?}/{SearchTitle?}")]
        public IActionResult Topics(TopicFiltersViewModel topicFilters) 
        {
            return View(topicFilters);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_TypeOfProject");
            }

            return View();
        }

        [Route("CreateSingleText")]
        public IActionResult CreateSingleText()
        {
            if (HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_CreateSingleText");
            }

            return View();
        }

        [Route("DefineSourceLang")]
        public IActionResult DefineSourceLang()
        {
            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_DefineSourceLang");
            }

            return View();
        }

        [Route("DefineTargetLang/{langCode}")]
        public IActionResult DefineTargetLang(string langCode)
        {
            if(HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_DefineTargetLang", langCode);
            }

            return View("DefineTargetLang", langCode);
        }

        [Route("CreateTopic/{langCode}")]
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
                    if(await _languageService.ExistsLangCode(langpar[0]) && await _languageService.ExistsLangCode(langpar[1]))
                    {
                        topic.SourceLang = langpar[0];
                        topic.TargetLang = langpar[1];
                    }
                }
            }
            else
            {
                if (await _languageService.ExistsLangCode(langCode))
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

        [HttpPost("CreateTopic/{langCode}")]
        public async Task<IActionResult> CreateTopic(TopicFieldViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Language sourceLang = await _languageService.GetLanguageByLangCodeAsync(model.SourceLang);
            
            if (sourceLang == null)
            {
                TempData["WrongResponse"] = "Chosen Culture is not found";
                return View(model);
            }

            SqlExpressions.GetTextTableName(model.SourceLang, out string sourceTableName);

            if (!await RawSqlQueryHelper.SpIfTableExists(_db, sourceTableName))
            {
                TempData["WrongResponse"] = "Chosen Culture is not supports";
                return View(model);
            }

            Language targetLang = null;

            if (!string.IsNullOrEmpty(model.TargetLang))
            {
                targetLang = await _languageService.GetLanguageByLangCodeAsync(model.TargetLang);
                if (targetLang == null)
                {
                    TempData["WrongResponse"] = "Chosen Culture is not found";
                    return View(model);
                }

                SqlExpressions.GetTextTableName(model.TargetLang, out string targetTableName);

                if (!await RawSqlQueryHelper.SpIfTableExists(_db, targetTableName))
                {
                    TempData["WrongResponse"] = "Chosen Target Culture is not supports";
                    return View(model);
                }
            }

            var currUser = await _userManager.GetUserAsync(User);

            if(currUser == null)
            {
                return View(model);
            }

            if (currUser.LimitOfText > 0)
            {
                IEnumerable<TopicsViewModel> topics = await _topicService.GetAllTopicsByLoggedInUser(currUser.Id);
                int amountOfText = await _textService.CalculateUserTopicsTextContentAsync(topics);

                int restOfTextVolume = currUser.LimitOfText - amountOfText;

                if ((restOfTextVolume - model.TopicTitle.Length) < 0)
                {
                    TempData["WrongResponse"] = _sharedLocalizer["MemoryLimitIsOver"].Value;
                    return View(model);
                }
            }

            Topic topic = new Topic
            {
                TopicTitle = model.TopicTitle,
                SourceLangId = sourceLang.Id,
                SourceLang = model.SourceLang,
                TargetLangId = targetLang?.Id,
                TargetLang = model.TargetLang,
                PublicText = false,
                UserId = currUser.Id,
                CreatedDateTime = DateTime.Now,
                AgeLimitation = model.AgeLimitation
            };

            Topic newTopic = await _topicService.CreateTopicAsync(topic);

            ResponseFromRawQuery sourcetext = await _textService.CreateTextAsync(newTopic.Id, model.SourceLang, newTopic.TopicTitle, model.NumberOfLines);
            string whatToEdit = "source";
            if (!string.IsNullOrEmpty(model.TargetLang))
            {
                ResponseFromRawQuery targettext = await _textService.CreateTextAsync(newTopic.Id, model.TargetLang, newTopic.TopicTitle, model.NumberOfLines);
                whatToEdit = string.Empty;
            }

            if (sourcetext != null && sourcetext.RespInt > 0)
            {
                return RedirectToAction(nameof(EditProject), new { id = newTopic.Id, whatToEdit });
                //return RedirectToAction(nameof(Topics));
            }

            return View(model);
        }

        [HttpPost("SearchCopyright")]
        public async Task<IActionResult> SearchCopyright(string copyright)
        {
            if(!string.IsNullOrEmpty(copyright))
            {
                int amountCopyrights = 5;
                var copyrightResults = new List<TextCopyrightModel>();
                copyrightResults = await _copyrightService.GetSomeFirstStartWithCopyrights(amountCopyrights, copyright);
                if (copyrightResults != null && copyrightResults.Count > 0)
                {
                    int countResult = copyrightResults.Count;
                    if (countResult < amountCopyrights)
                    {
                        var firstFoundCopyrights = new HashSet<int>(copyrightResults.Select(x => x.Id));
                        int howMuchMore = amountCopyrights - countResult;
                        var copyrightContains = await _copyrightService.GetSomeContainsExceptFoundCopyrights(howMuchMore, copyright, firstFoundCopyrights);
                        if(copyrightContains != null && copyrightContains.Count > 0)
                        {
                            foreach(var cprght in copyrightContains)
                            {
                                copyrightResults.Add(cprght);
                            }
                        }
                    }
                    return PartialView("_TextCopyright", copyrightResults);
                }
                else
                {
                    copyrightResults = await _copyrightService.GetSomeFirstContainsCopyrights(amountCopyrights, copyright);
                    if(copyrightResults != null && copyrightResults.Count > 0)
                        return PartialView("_TextCopyright", copyrightResults);
                }
            }

            return Content("No matches");
        }

        [HttpPost("EditCopyright")]
        public async Task<IActionResult> EditCopyright(EditTextCopyright copyright)
        {
            string returnLocalUrl = "";
            string respMes = "";

            if(!HttpContext.Request.IsAjaxRequest())
            {
                HttpContext.Request.GetLocalReturnUrl(out string returnUrl);
                returnLocalUrl = returnUrl;
            }

            if (!ModelState.IsValid)
            {
                respMes = "Something went wrong.";
                if (string.IsNullOrEmpty(returnLocalUrl))
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                    return LocalRedirect(returnLocalUrl);
                }
            }

            var topic = await _db.Topics.FindAsync(copyright.TopicId);
            if (topic == null)
            {
                respMes = "Project not found.";
                if (string.IsNullOrEmpty(returnLocalUrl))
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                    return LocalRedirect(returnLocalUrl);
                }
            }

            // If the identifier is null, then need to create a new Copyright entity
            if (copyright.CopyrightId == null)
            {
                // Check now the copyright name without ToUpperInvariant
                var copyrightExists = await _copyrightService.GetTextCopyrightByCopyrightName(copyright.CopyrightName);
                if (copyrightExists != null)
                {
                    copyright.CopyrightId = copyrightExists.Id;
                }
                else
                {
                    var createdCopyright = await _copyrightService.CreateTextCopyright(copyright.CopyrightName);
                    if (createdCopyright == null)
                    {
                        respMes = "Can not to create copyright.";
                    }
                    else
                    {
                        copyright.CopyrightId = createdCopyright.Id;
                    }
                }
            }

            if (!string.IsNullOrEmpty(respMes))
            {
                if (string.IsNullOrEmpty(returnLocalUrl))
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                    return LocalRedirect(returnLocalUrl);
                }
            }

            if (copyright.CopyrightId != null)
            {
                int? idToTest = null;
                if(topic.CopyrightId != null)
                {
                    // If the identifiers are the sdame, then there is no need to update the record.
                    if (topic.CopyrightId == copyright.CopyrightId)
                    {
                        respMes = "OK! 💾";
                        if (string.IsNullOrEmpty(returnLocalUrl))
                        {

                            return Json(JsonResponseFactory.SuccessResponseMessage(respMes));
                        }
                        else
                        {
                            TempData["SuccessResponse"] = respMes;
                            return LocalRedirect(returnLocalUrl);
                        }
                    }
                    else
                    {
                        idToTest = topic.CopyrightId;
                    }
                }

                try
                {
                    topic.CopyrightId = copyright.CopyrightId;
                    _db.Update(topic);
                    await _db.SaveChangesAsync();

                    if(idToTest != null)
                    {
                        // If the copyright doesn't use with other project, then remove it.
                        if(!await _copyrightService.ThisCopyrightNameIsUsedMore(topic.Id, (int)idToTest))
                        {
                            await _copyrightService.DeleteTextCopyright((int)idToTest);
                        }
                    }

                    respMes = "The Copyright was successfully saved! 💾";
                    var respObject = new { copyrightId = copyright.CopyrightId, message = respMes };

                    if (string.IsNullOrEmpty(returnLocalUrl))
                    {

                        return Json(JsonResponseFactory.SuccessResponseObject(respObject));
                    }
                    else
                    {
                        TempData["SuccessResponse"] = respMes;
                        return LocalRedirect(returnLocalUrl);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Unexpected error ocured when connected Copyright Name: '{ex.Message}'.");
                    respMes = "Unexpected error ocured when connected Copyright Name.";

                    if (string.IsNullOrEmpty(returnLocalUrl))
                    {
                        return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                    }
                    else
                    {
                        TempData["WrongResponse"] = respMes;
                        return LocalRedirect(returnLocalUrl);
                    }
                }
            }

            respMes = "Something went wrong.";
            if (string.IsNullOrEmpty(returnLocalUrl))
            {
                return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
            }
            else
            {
                TempData["WrongResponse"] = respMes;
                return LocalRedirect(returnLocalUrl);
            }

        }

        [HttpPost("EditTitle")]
        public async Task<IActionResult> EditTitle(int? TopicId, string TopicTitle)
        {
            string returnLocalUrl = "";
            string respMes = "";

            if(!HttpContext.Request.IsAjaxRequest())
            {
                HttpContext.Request.GetLocalReturnUrl(out string returnUrl);
                returnLocalUrl = returnUrl;
            }
            
            if (string.IsNullOrEmpty(TopicTitle) || TopicId == null)
            {
                respMes = "Title cannot be empty.";
                if (string.IsNullOrEmpty(returnLocalUrl))
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                }
            }
            else
            {
                var topic = await _db.Topics.FindAsync(TopicId);
                if(topic != null)
                {
                    try
                    {
                        topic.TopicTitle = TopicTitle;
                        _db.Update(topic);
                        await _db.SaveChangesAsync();

                        respMes = "Title was successfully updated!";
                        if (string.IsNullOrEmpty(returnLocalUrl))
                        {
                            return Json(JsonResponseFactory.SuccessResponseMessage(respMes));
                        }
                        else
                        {
                            TempData["SuccessResponse"] = respMes;
                            return LocalRedirect(returnLocalUrl);
                        }
                    }
                    catch(DbUpdateConcurrencyException ex)
                    {
                        _logger.LogError(ex, $"Unexpected error ocured when updated a Project Title: '{ex.Message}'.");
                        respMes = "Unexpected error ocured when updated a Project Title.";

                        if (string.IsNullOrEmpty(returnLocalUrl))
                        {
                            return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                        }
                        else
                        {
                            TempData["WrongResponse"] = respMes;
                            return LocalRedirect(returnLocalUrl);
                        }
                    }
                }
                else
                {
                    respMes = "Project not found.";
                    if (string.IsNullOrEmpty(returnLocalUrl))
                    {
                        return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                    }
                    else
                    {
                        TempData["WrongResponse"] = respMes;
                        return LocalRedirect(returnLocalUrl);
                    }
                }
            }

            respMes = "Something went wrong.";
            if (string.IsNullOrEmpty(returnLocalUrl))
            {
                return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
            }
            else
            {
                TempData["WrongResponse"] = respMes;
                return LocalRedirect(returnLocalUrl);
            }
        }

        [Route("ProjectText/{Id?}/{HowToDisplay?}")]
        public async Task<IActionResult> ProjectText(int? Id, string HowToDisplay = "")
        {
            if(Id == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            ProjectWithText projecttext = await _topicService.GetTopicWithTextById(Id, HowToDisplay);

            if (projecttext == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            ProjectTextViewModel projecttexttoview = TextMapping.ProjectTextToView(projecttext);

            if (HttpContext.Request.IsAjaxRequest())
            {
                switch (HowToDisplay)
                {
                    case "general":
                        return PartialView("Partial/_SourceAndTargetDisplay", projecttexttoview);
                    case "source":
                        return PartialView("Partial/_SourceOrTargetDisplay", projecttexttoview.SourceText);
                    case "target":
                        return PartialView("Partial/_SourceOrTargetDisplay", projecttexttoview.TargetText);
                    default:
                        return PartialView("Partial/_ModalProjectText", projecttexttoview);
                }
            }

            return View(projecttexttoview);
        }

        [Route("Practice/{Id?}/{HowToDisplay?}")]
        public async Task<IActionResult> Practice(int? Id, string HowToDisplay = "")
        {
            if (Id == null)
            {
                TempData["WrongResponse"] = "No such project was found";
                return RedirectToAction("Index", "Home");
            }

            // In this result is imposible that HowToDisplay value will be other than viceversa
            if (!string.IsNullOrEmpty(HowToDisplay) && HowToDisplay != "viceversa")
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            string[] compareUrl = new string[] { "Projects", "Practice" };
            HttpContext.Request.GetLocalOrSessionReturnUrl(compareUrl, out string returnUrl);

            ProjectWithText projecttext = await _topicService.GetTopicWithTextById(Id, HowToDisplay);

            if(projecttext == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            ProjectTextViewModel projecttexttoview = TextMapping.ProjectTextToView(projecttext);
            projecttexttoview.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Projects") : returnUrl;

            return View(projecttexttoview);
        }

        [Route("PracticeSync/{Id?}/{HowToDisplay?}")]
        public async Task<IActionResult> PracticeSync(int? Id, string HowToDisplay = "")
        {
            if(Id == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            // In this result is imposible that HowToDisplay value will be other than viceversa
            if (!string.IsNullOrEmpty(HowToDisplay) && HowToDisplay != "viceversa")
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Request.GetLocalReturnUrl(out string returnUrl);

            ProjectWithText projecttext = await _topicService.GetTopicWithTextById(Id, HowToDisplay);

            if (projecttext == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            PracticeSyncViewModel practice = TextMapping.ProjectToView(projecttext);
            practice.SentenceIndex = 0;
            practice.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Projects") : returnUrl;

            return View(practice);
        }

        [HttpPost]
        public async Task<IActionResult> PracticeSync(
            int? TopicId, int? SentenceIndex, string Answer = "", 
            string HowToDisplay = "", string CorrectAnswerIndexes = "", string ReturnUrl = "")
        {
            if (TopicId == null)
            {
                TempData["WrongResponse"] = _sharedLocalizer["NoSuchProjectWasFound"];
                return RedirectToAction("Index", "Home");
            }

            // In this result is imposible that HowToDisplay value will be other than viceversa
            if (!string.IsNullOrEmpty(HowToDisplay) && HowToDisplay != "viceversa")
            {
                TempData["WrongResponse"] = _sharedLocalizer["NoSuchProjectWasFound"];
                return RedirectToAction("Index", "Home");
            }

            ProjectWithText projecttext = await _topicService.GetTopicWithTextById(TopicId, HowToDisplay);

            if (projecttext == null)
            {
                TempData["WrongResponse"] = _sharedLocalizer["NoSuchProjectWasFound"];
                return RedirectToAction("Index", "Home");
            }

            PracticeSyncViewModel practice = TextMapping.ProjectToView(projecttext);
            practice.CorrectAnswerIndexes = CorrectAnswerIndexes;
            practice.SentenceIndex = SentenceIndex;
            practice.Answer = Answer;
            practice.ReturnUrl = ReturnUrl;

            if (!string.IsNullOrEmpty(Answer))
            {
                string correctText = practice.SourceText.ElementAt((int)SentenceIndex).Sentence;
                
                int loopLength = 0;
                string restOfText = "";
                bool errorInAnswer = false;

                // Tag building strings
                string startSuccessTag = "<span class='text-success'>";
                string startMutedTag = "<span class='text-muted'>";
                string startWrongTag = "<span class='text-danger'>";
                string endSpanTag = "</span>";

                if (Answer.Length < correctText.Length)
                {
                    loopLength = Answer.Length;
                    restOfText = startMutedTag + $"<i> {_sharedLocalizer["OBSSentenceNotComplete"]}</i>" + endSpanTag;
                    errorInAnswer = true;
                }
                else if(Answer.Length > correctText.Length)
                {
                    loopLength = correctText.Length;
                    restOfText = Answer.Substring(loopLength - 1);
                }

                bool wrongChar = false;
                string responseText = "";

                for(int i = 0; i < loopLength; i++)
                {
                    if (!correctText[i].Equals(Answer[i]))
                    {
                        errorInAnswer = true;
                        if (!wrongChar)
                        {
                            responseText += i > 0 ? endSpanTag + startWrongTag + Answer[i] : startWrongTag + Answer[i];
                        }
                        else
                        {
                            responseText += Answer[i];
                        }

                        // Close tag if the char is last
                        if (loopLength - 1 == i)
                        {
                            responseText += Answer.Length < correctText.Length ? endSpanTag + startMutedTag + $"<i> {_sharedLocalizer["OBSSentenceNotComplete"]}</i>" + endSpanTag : restOfText + endSpanTag;
                        }

                        wrongChar = true;
                    }
                    else
                    {
                        if(wrongChar)
                        {
                            // Close the char was wrong
                            responseText += endSpanTag + startSuccessTag + Answer[i] + (loopLength - 1 == i ? endSpanTag : "");
                            wrongChar = false;
                        }
                        else
                        {
                            responseText += i == 0 ? 
                                startSuccessTag + Answer[i] + (loopLength - 1 == i ? endSpanTag + startWrongTag + restOfText + endSpanTag : "") :
                                Answer[i] + (loopLength - 1 == i ? endSpanTag + startWrongTag + restOfText + endSpanTag : "");                        
                        }
                    }
                }

                practice.ResponseText = responseText;
                HashSet<string> CorrectAnswersIndexesHashSet = new HashSet<string>();

                if (!string.IsNullOrEmpty(CorrectAnswerIndexes))
                {
                    string[] indexes = CorrectAnswerIndexes?.Split(',');
                    if (indexes != null && indexes.Length > 0)
                    {
                        foreach (var index in indexes)
                        {
                            CorrectAnswersIndexesHashSet.Add(index);
                        }
                    }
                }

                if (errorInAnswer)
                {
                    if (CorrectAnswersIndexesHashSet != null && CorrectAnswersIndexesHashSet.Contains(SentenceIndex.ToString()))
                    {
                        CorrectAnswersIndexesHashSet.Remove(SentenceIndex.ToString());
                    }
                }
                else
                {
                    if(SentenceIndex != null)
                    {
                        // Save index in the HashSet
                        if (CorrectAnswersIndexesHashSet != null)
                        {
                            if (!CorrectAnswersIndexesHashSet.Contains(SentenceIndex.ToString()))
                            {
                                CorrectAnswersIndexesHashSet.Add(SentenceIndex.ToString());
                            }
                        }
                        else
                        {
                            CorrectAnswersIndexesHashSet.Add(SentenceIndex.ToString());
                        }
                        // Save index in the HashSet
                        practice.CorrectAnswerIndexes = string.Join(",", CorrectAnswersIndexesHashSet);
                    }

                    practice.Answer = "";

                    int nextSentenceIndex = (int)SentenceIndex + 1;
                    
                    if(nextSentenceIndex < practice.SourceText.Count() && 
                        (CorrectAnswersIndexesHashSet != null && !CorrectAnswersIndexesHashSet.Contains(nextSentenceIndex.ToString())))
                    {
                        practice.SentenceIndex = nextSentenceIndex;
                    }
                    else
                    {
                        practice.SentenceIndex = null;
                    }
                }

                practice.CorrectAnswersIndexesHashSet = CorrectAnswersIndexesHashSet;
            }
            else
            {
                if(!string.IsNullOrEmpty(CorrectAnswerIndexes))
                {
                    string[] indexes = CorrectAnswerIndexes?.Split(',');
                    if (indexes != null && indexes.Length > 0)
                    {
                        HashSet<string> CorrectAnswersIndexesHashSet = new HashSet<string>();
                        foreach (var index in indexes)
                        {
                            CorrectAnswersIndexesHashSet.Add(index);
                        }
                        practice.CorrectAnswersIndexesHashSet = CorrectAnswersIndexesHashSet;
                    }
                }    
            }

            return View(practice);
        }

        [Route("PublishProject/{id?}/{viewMode?}")]
        public async Task<IActionResult> PublishProject(int? id, string viewMode = "")
        {
            bool success = false;
            string wrongRespStr = _sharedLocalizer["ChangesFailed"].Value;
            string successRespStr = _sharedLocalizer["ChangesMadeSuccessfully"].Value + "!";
            if (id != null)
            {
                Topic topic = await _db.Topics.FindAsync(id);

                if (topic != null)
                {
                    if (topic.PublicText == true)
                    {
                        topic.PublicText = false;
                    }
                    else
                    {
                        topic.PublicText = true;
                        successRespStr = _sharedLocalizer["PublishedSuccessfully"].Value + "!";
                    }

                    try
                    {
                        _db.Update(topic);
                        await _db.SaveChangesAsync();
                        success = true;
                    }
                    catch (DbUpdateConcurrencyException /*ex*/)
                    {
                        TempData["WrongResponse"] = wrongRespStr;
                    }
                }
            }

            if (HttpContext.Request.IsAjaxRequest())
            {
                JsonResponceToAlertBox jsonResponse = new JsonResponceToAlertBox();

                if (success)
                {
                    jsonResponse.AlertClasses = "alert-success";
                    jsonResponse.ResponseText = successRespStr;
                }
                else
                {
                    jsonResponse.AlertClasses = "alert-danger";
                    jsonResponse.ResponseText = wrongRespStr;
                    //jsonResponse.ResponseText = responceModel.RespStr;
                }

                return Json(jsonResponse);
            }

            if (success)
            {
                TempData["SuccessResponse"] = successRespStr;
            }
            else
            {
                TempData["WrongResponse"] = wrongRespStr;
            }

            return RedirectToAction(nameof(Topics), new { viewMode });
        }

        [HttpGet("EditProject/{Id?}/{WhatToEdit?}")]
        public async Task<ActionResult> EditProject(int? id, string WhatToEdit = "")
        {
            if (id == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            ProjectWithText projecttext = await _topicService.GetTopicWithTextById(id);

            if(projecttext == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            var currUser = await _userManager.GetUserAsync(User);

            if (projecttext.ProjectAuthorId != currUser.Id)
            {
                TempData["WrongResponse"] = "You have not right to edit this project.";
                return RedirectToAction("Index", "Home");
            }

            EditProjectViewModel projecttoview = TextMapping.EditProjectToView(projecttext, WhatToEdit);
            projecttoview.WhatToEdit = WhatToEdit;

            // ReturnUrl
            string[] uriArr = new Uri(HttpContext.Request.Headers["Referer"]).LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (uriArr.Length > 1 && ((uriArr[0] == "Projects" && uriArr[1] == "EditProject") || uriArr[0] == "ManageTags"))
            {
                string sessionString = HttpContext.Session.GetString("ReturnUrl");
                if (sessionString != null || !string.IsNullOrEmpty(sessionString))
                {
                    projecttoview.ReturnUrl = sessionString;
                }
                else
                {
                    projecttoview.ReturnUrl = "/";
                }
            }
            else
            {
                if (uriArr.Length > 1 && (uriArr[0] == "Projects" && uriArr[1] == "Create"))
                {
                    string returnUrl = Url.Action("Topics", "Projects", new { viewMode = "UserTopics" });
                    projecttoview.ReturnUrl = returnUrl;
                    HttpContext.Session.SetString("ReturnUrl", returnUrl);
                }
                else
                {
                    HttpContext.Request.GetLocalReturnUrl(out string returnUrl);
                    projecttoview.ReturnUrl = returnUrl;
                    HttpContext.Session.SetString("ReturnUrl", returnUrl);
                }
            }

            return View(projecttoview);
        }

        [HttpPost("EditProject/{Id?}/{WhatToEdit?}")]
        public async Task<IActionResult> EditProject(EditSentence model)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Request.GetLocalReturnUrl(out string ReturnUrl);

                var currUser = await _userManager.GetUserAsync(User);

                if(currUser == null)
                {
                    TempData["WrongResponse"] = _sharedLocalizer["ChangesFailed"].Value;
                    return LocalRedirect(ReturnUrl);
                }

                ResponseFromRawQuery responseModel = new ResponseFromRawQuery();

                if (model.ActionBtn == "edit")
                {
                    int? restOfTextVolume = null;

                    if (string.IsNullOrEmpty(model.Sentence))
                    {
                        model.Sentence = TextHelpers.DefSentence;
                    }
                    else
                    {
                        if(!HttpContext.Request.IsAjaxRequest())
                        {
                            if(model.Sentence.Length > 250)
                            {
                                TempData["WrongResponse"] = _sharedLocalizer["InputRestriction"].Value;
                                return LocalRedirect(ReturnUrl);
                            }
                            else
                            {
                                if(currUser.LimitOfText > 0)
                                {
                                    IEnumerable<TopicsViewModel> topics = await _topicService.GetAllTopicsByLoggedInUser(currUser.Id);
                                    if(!topics.Any(x => x.TopicId == model.TopicId))
                                    {
                                        TempData["WrongResponse"] = _sharedLocalizer["ChangesFailed"].Value;
                                        return LocalRedirect(ReturnUrl);
                                    }

                                    int amountOfText = await _textService.CalculateUserTopicsTextContentAsync(topics);

                                    restOfTextVolume = currUser.LimitOfText - amountOfText;
                                }
                            }
                                
                        }
                    }

                    responseModel = await _textService.EditSentenceAsync(model, restOfTextVolume);

                    if (HttpContext.Request.IsAjaxRequest())
                    {
                        JsonResponceToAlertBox jsonResponse = new JsonResponceToAlertBox();

                        if (responseModel != null && responseModel.RespInt > 0)
                        {
                            jsonResponse.AlertClasses = "alert-success";
                            jsonResponse.ResponseText = _sharedLocalizer["ChangesMadeSuccessfully"].Value + "!";
                            //jsonResponse.ResponseText = _sharedLocalizer["ChangesMadeSuccessfully"].Value + " " + responseModel.RespInt;
                        }
                        else
                        {
                            jsonResponse.AlertClasses = "alert-danger";
                            jsonResponse.ResponseText = _sharedLocalizer["ChangesFailed"].Value;
                            //jsonResponse.ResponseText = responceModel.RespStr;
                        }

                        return Json(jsonResponse);
                    }
                }

                ProjectWithText projecttext = await _topicService.GetTopicWithTextById(model.TopicId);

                if (model.ActionBtn == "delete")
                {
                    UpdateSentenceFormText delSntnceFromTxts = TextMapping.TextsToDeleteSentence(projecttext, model.TargetTextIndex);
                    responseModel = await _textService.DeleteSentenceAsync(delSntnceFromTxts);
                }

                if (responseModel != null && responseModel.RespInt > 0)
                {
                    TempData["SuccessResponse"] = _sharedLocalizer["ChangesMadeSuccessfully"].Value + "!";
                    //TempData["SuccessResponse"] = _sharedLocalizer["ChangesMadeSuccessfully"].Value + " (" + responseModel.RespInt + " objects).";
                }
                else
                {
                    TempData["WrongResponse"] = _sharedLocalizer["ChangesFailed"].Value;
                    //jsonResponse.ResponseText = responceModel.RespStr;
                }

                if (model.ActionBtn == "delete")
                {
                    return LocalRedirect(ReturnUrl);
                }
                else
                {
                    return LocalRedirect(ReturnUrl);
                }
            }

            TempData["WrongResponse"] = "Wrong Request!";
            return LocalRedirect(HttpContext.Session.GetString("ReturnUrl"));
        }
        [HttpPost("AddLinesToProject")]
        public async Task<IActionResult> AddLinesToProject(AddLinesToText model)
        {
            if(ModelState.IsValid)
            {
                ResponseFromRawQuery commonResponse = new ResponseFromRawQuery();
                Topic topic = await _db.Topics.FindAsync(model.TopicId);

                if(topic != null)
                {
                    if(!string.IsNullOrEmpty(topic.SourceLang))
                    {
                        TextHelpers.MakeAdditionalLines(model.NumberOfLines, out string additionalLines);
                        SqlExpressions.GetTextTableName(topic.SourceLang, out string textPrimaryTableName);

                        if (topic.SourceLang == topic.TargetLang)
                        {
                            List<TextContentWithId> textContents = await _textService.GetTextContentByLangCodeAndTopicId(topic.SourceLang, topic.Id);
                            if (textContents != null && textContents.Count() > 0)
                            {
                                foreach (var textContent in textContents)
                                {
                                    string increasedTextContent = string.IsNullOrEmpty(textContent.TextContent) ? textContent.TextContent + additionalLines :
                                        textContent.TextContent + TextHelpers.RTSeparator + additionalLines;
                                    var response = await RawSqlQueryHelper.RawUpdateTextContent(_db, increasedTextContent, textPrimaryTableName, textContent.Id);
                                    if (response != null)
                                    {
                                        commonResponse.RespInt = response.RespInt;
                                        commonResponse.RespStr = response.RespStr;
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<TextContentWithId> sourceTextContent = await _textService.GetTextContentByLangCodeAndTopicId(topic.SourceLang, topic.Id);
                            if (sourceTextContent != null && sourceTextContent.Count() > 0)
                            {
                                string currentSourceTextContent = sourceTextContent.First().TextContent;
                                string sourceIncreasedTextContent = string.IsNullOrEmpty(currentSourceTextContent) ? currentSourceTextContent + additionalLines :
                                    currentSourceTextContent + TextHelpers.RTSeparator + additionalLines;
                                var response = await RawSqlQueryHelper.RawUpdateTextContent(_db, sourceIncreasedTextContent, textPrimaryTableName, sourceTextContent.First().Id);
                                if (response != null)
                                {
                                    commonResponse.RespInt = response.RespInt;
                                    commonResponse.RespStr = response.RespStr;
                                }
                            }

                            if (!string.IsNullOrEmpty(topic.TargetLang))
                            {
                                List<TextContentWithId> targetTextContent = await _textService.GetTextContentByLangCodeAndTopicId(topic.TargetLang, topic.Id);
                                if (targetTextContent != null && targetTextContent.Count() > 0)
                                {
                                    string currentTargetTextContent = targetTextContent.First().TextContent;
                                    string targetIncreasedTextContent = string.IsNullOrEmpty(currentTargetTextContent) ? currentTargetTextContent + additionalLines :
                                        currentTargetTextContent + TextHelpers.RTSeparator + additionalLines;
                                    SqlExpressions.GetTextTableName(topic.TargetLang, out string textSecondaryTableName);
                                    var response = await RawSqlQueryHelper.RawUpdateTextContent(_db, targetIncreasedTextContent, textSecondaryTableName, targetTextContent.First().Id);
                                    if (response != null)
                                    {
                                        commonResponse.RespInt = response.RespInt;
                                        commonResponse.RespStr = response.RespStr;
                                    }
                                }
                            }
                        }
                    }
                }

                if(string.IsNullOrEmpty(commonResponse.RespStr) || commonResponse.RespInt == 0)
                {
                    TempData["WrongResponse"] = _sharedLocalizer["ChangesFailed"].Value;
                }
                else
                {
                    TempData["SuccessResponse"] = commonResponse.RespStr;
                }

                return RedirectToAction(nameof(EditProject), new { id = model.TopicId, WhatToEdit = model.WhatToEdit });
            }

            TempData["WrongResponse"] = _sharedLocalizer["ChangesFailed"].Value;

            return RedirectToAction("Index", "Home");
        }
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            string[] compareUrl = new string[] { "Projects", "Delete" };
            HttpContext.Request.GetLocalOrSessionReturnUrl(compareUrl, out string returnUrl);
            
            if (id != null)
            {
                // There it removes tags and the copyright, if necessary
                var response = await _topicService.DeleteTopicAsync(HttpContext, id);

                if (response.RespInt <= 0)
                {
                    TempData["WrongResponse"] = response.RespStr;
                }
                else
                {
                    TempData["SuccessResponse"] = response.RespStr;
                }
            }
            
            return LocalRedirect(returnUrl);
        }
    }
}
