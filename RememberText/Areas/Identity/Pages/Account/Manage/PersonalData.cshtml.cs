using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RememberText.Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using RememberText.RTTools;

namespace RememberText.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            UserManager<User> userManager,
            ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public string Nickname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //ValidateHelpers.IsLocalReturnUrl(Request, out string returnUrl);
            
            Nickname = user.Nickname;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;

            return Page();
        }

        
    }
}