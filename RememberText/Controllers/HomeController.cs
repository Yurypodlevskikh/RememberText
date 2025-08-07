using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.Models;
using RememberText.RTTools.Filters;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using UAParser;

namespace RememberText.Controllers
{
    [ServiceFilter(typeof(FirstIpAddressFilter))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IRTTopicService _topicService;
        private readonly IRTGuestBookService _guestBookService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, 
            IStringLocalizer<SharedResource> sharedLocalizer,
            IRTTopicService topicService,
            IRTGuestBookService guestBookService,
            UserManager<User> userManager,
            IEmailSender emailSender,
            IConfiguration config)
        {
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _topicService = topicService;
            _userManager = userManager;
            _guestBookService = guestBookService;
            _emailSender = emailSender;
        }

        [Route("{ViewMode:alpha?}/{TagParam?}")]
        [Route("{ViewMode:alpha?}/{SortOrder?}/{PageNumber:int?}/{TagParam?}/{SearchTitle?}")]
        public IActionResult Index(TopicFiltersViewModel topicFilters)
        {
            return View(topicFilters);
        }

        [Route("Home/Message")]
        public async Task<IActionResult> Message()
        {
            string[] compareUrl = new string[] { "Home", "Message" };
            HttpContext.Request.GetLocalOrSessionReturnUrl(compareUrl, out string returnUrl);

            User currentUser = await _userManager.GetUserAsync(User);

            if(currentUser != null)
            {
                var guestBook = new SendGuestBookViewModel
                {
                    Nickname = currentUser.Nickname
                };

                return View(guestBook);
            }

            TempData["WrongResponse"] = _sharedLocalizer["SendMessageNeedLogin"].Value;
            return RedirectToPage("/Account/Login", new { area = "Identity" });
            //return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpPost]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        [Route("Home/Message")]
        public async Task<IActionResult> Message([Bind("Nickname,Title,Message")] SendGuestBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get return url from session
                string returnUrl = HttpContext.Session.GetString("ReturnUrl");

                User currentUser = await _userManager.GetUserAsync(User);

                if(currentUser != null)
                {
                    var guestBookEntry = new GuestBookEntry
                    {
                        UserId = currentUser.Id,
                        MessageTitle = model.Title,
                        Message = model.Message,
                        CreatedDateTime = DateTime.Now
                    };

                    await _guestBookService.AddMessage(guestBookEntry);
                    var callback = Url.Action("Details", "GuestBookEntries");
                    ViewBag.Callback = $"<a href='https://yurkinsson.com{HtmlEncoder.Default.Encode(callback)}'>here</a>";

                    var messageUrl = Url.Action("Details", "GuestBookEntries", new { id = guestBookEntry.Id });
                    string message = $@"{currentUser.Nickname} sent a message. 
                        Click <a href='{HtmlEncoder.Default.Encode(messageUrl)}'>here</a> to show a message.";
                    await _emailSender.SendEmailAsync("yurkinsson@gmail.com", "A new message has arrived", message);

                    return LocalRedirect(returnUrl);
                }
            }
            
            return View(model);
        }

        [Route("Home/Introduction")]
        public IActionResult Introduction()
        {
            TopicFiltersViewModel topicFilters = new TopicFiltersViewModel();
            topicFilters.ViewMode = "PublicTopics";
            return View(topicFilters);
        }

        [HttpPost("Home/SetLanguage")]
        public IActionResult SetLanguage(string culture)
        {
            HttpContext.Request.GetLocalReturnUrl(out string returnUrl);
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
        }

        [Route("Home/ProjectText/{Id?}/{HowToDisplay?}")]
        public async Task<IActionResult> ProjectText(int? Id, string HowToDisplay = "")
        {
            if (Id == null)
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
                        return PartialView("Partial/_ModalPublicProjectText", projecttexttoview);
                }
            }

            return View(projecttexttoview);
        }

        [Route("Home/Practice/{Id?}/{HowToDisplay?}")]
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

            string[] compareUrl = new string[] { "Home", "Practice" };
            HttpContext.Request.GetLocalOrSessionReturnUrl(compareUrl, out string returnUrl);

            ProjectWithText projecttext = await _topicService.GetTopicWithTextById(Id, HowToDisplay);

            if (projecttext == null)
            {
                TempData["WrongResponse"] = "No such project was found.";
                return RedirectToAction("Index", "Home");
            }

            ProjectTextViewModel projecttexttoview = TextMapping.ProjectTextToView(projecttext);
            projecttexttoview.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Projects") : returnUrl;

            return View(projecttexttoview);
        }

        [HttpGet("Home/PracticeSync/{Id?}/{HowToDisplay?}")]
        public async Task<IActionResult> PracticeSync(int? Id, string HowToDisplay = "")
        {
            if (Id == null)
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

        [HttpPost("Home/PracticeSync/{Id?}/{HowToDisplay?}")]
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

                for (int i = 0; i < loopLength; i++)
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
                        if (wrongChar)
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
                    if (SentenceIndex != null)
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

                    if (nextSentenceIndex < practice.SourceText.Count() &&
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
                if (!string.IsNullOrEmpty(CorrectAnswerIndexes))
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

        [Route("Home/Donate")]
        public IActionResult Donate()
        {
            return View();
        }

        [HttpGet("Home/Privace")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("Home/Why")]
        public IActionResult Why()
        {
            return View();
        }

        [HttpGet("Home/How")]
        public IActionResult How()
        {
            return View();
        }

        [HttpGet("Home/About")]
        public IActionResult About()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
