using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RememberText.Infrastructure.Interfaces;
using RememberText.RTTools.Filters;
using RememberText.ViewModels;
using System.Data;
using System.Linq;

namespace RememberText.Controllers
{
    [Authorize(Roles = "GenAdministrator")]
    [Route("IpAddresses")]
    public class IpAddressesController : Controller
    {
        private readonly IIpAddressesData _ipAddressesData;

        public IpAddressesController(IIpAddressesData ipAddressesData)
        {
            _ipAddressesData = ipAddressesData;
        }

        // GET: IpAddressesController
        [Route("Index")]
        public IActionResult Index() => View(_ipAddressesData.GetIpAddresses().Select(x => new IpAddressesViewModel
        {
            Id = x.Id,
            Ip = x.Ip,
            CountryName = x.Region,
            City = x.City,
            CreatedDateTime = x.CreatedDateTime
        }));

        // GET: IpAddressesController/Details/5
        public IActionResult Details(int id)
        {
            var ipaddress = _ipAddressesData.GetIpAddressById(id);

            return ipaddress is null ? NotFound() : (IActionResult)View(ipaddress);

            // From Home/Index
            //string ipAddress = "None";
            //if (!_cache.TryGetValue("83.248.115.115", out ipAddress))
            //{

            //}

            //ViewBag.CacheIpAddress = ipAddress;

            //ViewBag.ipadress = ValidateHelpers.getIPAddress(_httpContextAccessor);

            //return View(TestData.IpAddresses);
        }
    }
}
