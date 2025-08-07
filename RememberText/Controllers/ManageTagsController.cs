using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class ManageTagsController : Controller
    {
        private readonly IRTNormalizedTagService _normalTagService;
        private readonly IRTTextService _textService;
        private readonly IRTTagService _tagService;
        private readonly IRTTagAssignmentService _tagAssignmentService;
        private readonly IRTLanguageService _languageService;
        private readonly ICompositeViewEngine _viewEngine;

        public ManageTagsController(
            IRTNormalizedTagService normalTagService,
            IRTTextService textService,
            IRTTagService tagService,
            IRTTagAssignmentService tagAssignmentService,
            IRTLanguageService languageService,
            ICompositeViewEngine viewEngine)
        {
            _normalTagService = normalTagService;
            _textService = textService;
            _tagService = tagService;
            _tagAssignmentService = tagAssignmentService;
            _languageService = languageService;
            _viewEngine = viewEngine;
        }

        public IActionResult ShowTagsVC(string viewName, string langCode, int? textId)
        {
            return ViewComponent("Tag", new { viewName, langCode, textId });
        }

        [HttpPost("ManageTags/FindTag")]
        public async Task<IActionResult> FindTag(string tagName, string langCode, int textId)
        {
            List<TagViewModel> tags = await _tagService.GetOfferTagsExceptTextId(tagName, langCode, textId);
            if(tags != null && tags.Count > 0)
            {
                return PartialView("Partial/_OfferTags", tags);
            }
            return Content("No tags");
        }

        [HttpPost("ManageTags/CreateTag")]
        public async Task<IActionResult> CreateTag(string TagName, int? TextId, string LangCode, int? TagId)
        {
            string[] compareUrl = new string[] { "ManageTags", "CreateTag" };
            HttpContext.Request.GetLocalOrSessionReturnUrl(compareUrl, out string returnUrl);

            string respMes = "";

            if (string.IsNullOrEmpty(TagName))
            {
                respMes = "Tag Name is required";
            }

            if (TextId == null)
            {
                respMes = "Wrong Text reference";
            }

            if (string.IsNullOrEmpty(LangCode) ||!await _languageService.ExistsLangCode(LangCode))
            {
                respMes = "Wrong Culture";
            }

            if(!string.IsNullOrEmpty(respMes))
            {
                if(HttpContext.Request.IsAjaxRequest())
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                    //return Content(respMes);
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                    return LocalRedirect(returnUrl);
                }
            }

            if (TagId == null)
            {
                LangCode.ToUpperFirstLangTwoChars(out string langSubtag);
                TagId = await _tagService.GetTagId(langSubtag, TagName);

                if (TagId == null)
                {
                    // Create Normalized Tag Name
                    int? normalTagId = await _normalTagService.CreateNormTagAsync(TagName);

                    if (normalTagId == null || !await _textService.ExistsText((int)TextId, LangCode))
                    {
                        respMes = @"Unable to save changes. Try again, and if 
                                    the problem persists see your system administrator.";

                        if(HttpContext.Request.IsAjaxRequest())
                        {
                            return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                        }
                        else
                        {
                            TempData["WrongResponse"] = respMes;
                            return LocalRedirect(returnUrl);
                        }
                    }

                    TagId = await _tagService.CreateTagAsync((int)normalTagId, langSubtag, TagName);

                    if (TagId == null || TagId <= 0)
                    {
                        respMes = @"Unable to create this tag. Try again, and if 
                                    the problem persists see your system administrator.";

                        if (HttpContext.Request.IsAjaxRequest())
                        {
                            return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                        }
                        else
                        {
                            TempData["WrongResponse"] = respMes;
                            return LocalRedirect(returnUrl);
                        }
                    }
                }
            }
            
            if(TagId != null)
            {
                LangCode.ToUpperFirstLangTwoChars(out string langSubtag);
                string tagAssignmentResult = await _tagAssignmentService.CreateTagAssignmentAsync((int)TextId, (int)TagId, langSubtag);

                if (!string.IsNullOrEmpty(tagAssignmentResult))
                {
                    respMes = tagAssignmentResult;

                    if (HttpContext.Request.IsAjaxRequest())
                    {
                        return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                    }
                    else
                    {
                        TempData["WrongResponse"] = respMes;
                        return LocalRedirect(returnUrl);
                    }
                }

                if (HttpContext.Request.IsAjaxRequest())
                {
                    List<TagWithDeleteViewModel> items = TagMapping.ToTagWithDeleteView(await _tagService.GetTagsByLangCodeAndTextId(LangCode, (int)TextId), (int)TextId);
                    PartialViewResult partialViewResult = PartialView("Partial/_TagWithDelete", items.OrderBy(x => x.TagName).ToList());
                    string viewContent = ConvertViewToString.ViewToString(this.ControllerContext, partialViewResult, _viewEngine);
                    return Json(JsonResponseFactory.SuccessResponseMessage(viewContent));
                }

                TempData["SuccessResponse"] = "The tag was successfully saved.";
            }
            
            return LocalRedirect(returnUrl);
        }

        [HttpPost("ManageTags/Delete")]
        public async Task<IActionResult> Delete(string LangCode, int? TagId, int? TextId)
        {
            string[] compareUrl = new string[] { "ManageTags", "Delete" };
            HttpContext.Request.GetLocalOrSessionReturnUrl(compareUrl, out string returnUrl);
            string respMes = "";
            
            if (TagId == null || string.IsNullOrEmpty(LangCode) || TextId == null || !await _languageService.ExistsLangCode(LangCode))
            {
                respMes = "Tag Not Found";
            }

            if(!string.IsNullOrEmpty(respMes))
            {
                if (HttpContext.Request.IsAjaxRequest())
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                    return LocalRedirect(returnUrl);
                }
            }

            string resultMes = await _tagService.DisconnectTag(LangCode, TagId, TextId);

            if (string.IsNullOrEmpty(resultMes))
            {
                respMes = "Tag Not Found";

                if (HttpContext.Request.IsAjaxRequest())
                {
                    return Json(JsonResponseFactory.ErrorResponseMessage(respMes));
                }
                else
                {
                    TempData["WrongResponse"] = respMes;
                    return LocalRedirect(returnUrl);
                }
            }
            else
            {
                if (HttpContext.Request.IsAjaxRequest())
                {
                    List<TagWithDeleteViewModel> items = TagMapping.ToTagWithDeleteView(await _tagService.GetTagsByLangCodeAndTextId(LangCode, (int)TextId), (int)TextId);
                    PartialViewResult partialViewResult = PartialView("Partial/_TagWithDelete", items.OrderBy(x => x.TagName).ToList());
                    string viewContent = ConvertViewToString.ViewToString(this.ControllerContext, partialViewResult, _viewEngine);
                    return Json(JsonResponseFactory.SuccessResponseMessage(viewContent));
                }
                else
                {
                    TempData["SuccessResponse"] = resultMes;
                    return LocalRedirect(returnUrl);
                }
            }
        }
    }
}
