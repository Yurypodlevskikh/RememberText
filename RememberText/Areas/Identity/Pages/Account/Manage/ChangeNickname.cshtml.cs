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
    public class ChangeNicknameModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ChangeNicknameModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "New nickname")]
            public string Nickname { get; set; }
        }

        private void LoadToView(User user)
        {
            Input = new InputModel { Nickname = user.Nickname };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("Not Found");
            }

            LoadToView(user);
            return Page();
        }
    }
}
