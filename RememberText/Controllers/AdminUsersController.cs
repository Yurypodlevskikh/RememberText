using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RememberText.Controllers
{
    [Authorize(Roles = "GenAdministrator")]
    [RequireHttps]
    [Route("AdminUsers")]
    public class AdminUsersController : Controller
    {
        //[Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
