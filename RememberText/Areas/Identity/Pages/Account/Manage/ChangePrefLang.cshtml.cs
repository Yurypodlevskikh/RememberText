using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RememberText.Domain.Entities.Identity;

namespace RememberText.Areas.Identity.Pages.Account.Manage
{
    public class ChangePrefLangModel : PageModel
    {
        public readonly UserManager<User> _userManager;
        public readonly SignInManager<User> _signInManager;

        public ChangePrefLangModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string Nickname { get; set; }
        public class InputModel
        {
            [Required]
            public string PrefLang { get; set; }
        }

        private void LoadPrefLangs(User user)
        {
            Nickname = user.Nickname;

            Input = new InputModel
            {
                PrefLang = user.PreffLang
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("Not Found");
            }

            LoadPrefLangs(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string[] PrefLang)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Not Found");
            }

            if(!ModelState.IsValid)
            {
                LoadPrefLangs(user);
                return Page();
            }

            string prefLangStr = PrefLang != null ? String.Join(",", PrefLang) : "";

            if(user.PreffLang != prefLangStr)
            {
                user.PreffLang = prefLangStr;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToPage("PersonalData");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            LoadPrefLangs(user);
            return Page();
        }
    }
}
