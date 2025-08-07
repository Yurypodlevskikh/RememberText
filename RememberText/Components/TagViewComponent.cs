using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class TagViewComponent : ViewComponent
    {
        private readonly IRTTagService _tagService;
        private readonly UserManager<User> _userManager;
        public TagViewComponent(IRTTagService tagService, UserManager<User> userManager)
        {
            _tagService = tagService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string viewName, string langCode, int? textId)
        {
            if(textId == null)
            {
                if(viewName == "UserLangTags")
                {
                    IEnumerable<TagAndTaggedTexts> items = await GetUserTags(langCode);

                    return View(viewName, items);
                }
                else
                {
                    return View("Default");
                }
            }
            else
            {
                List<TagWithDeleteViewModel> items = TagMapping.ToTagWithDeleteView(await GetItems(langCode, (int)textId), (int)textId);
                return View(viewName, items);
            }
        }
        private async Task<IEnumerable<TagViewModel>> GetItems(string langCode, int textId)
        {
            IEnumerable<TagViewModel> tagViewModel = await _tagService.GetTagsByLangCodeAndTextId(langCode, textId);
            return tagViewModel.OrderBy(x => x.TagName);
        }
        private async Task<IEnumerable<TagAndTaggedTexts>> GetUserTags(string langCode)
        {
            User user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
            IEnumerable<TagAndTaggedTexts> tagViewModel = await _tagService.GetTagsByLangCodeAndUserId(langCode, user.Id);
            return tagViewModel?.OrderBy(x => x.TagName);
        }
    }
}
