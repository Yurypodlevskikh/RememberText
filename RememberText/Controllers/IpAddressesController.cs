using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RememberText.Data;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using RememberText.RTTools.Filters;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Controllers
{
    [ServiceFilter(typeof(FirstIpAddressFilter))]
    public class IpAddressesController : Controller
    {
        private readonly IIpAddressesData _ipAddressesData;
        private readonly List<IpAddress> _IpAddresses = TestData.IpAddresses;

        public IpAddressesController(IIpAddressesData ipAddressesData)
        {
            _ipAddressesData = ipAddressesData;
        }

        // GET: IpAddressesController
        public IActionResult Index() => View(_ipAddressesData.GetIpAddresses().Select(x => new IpAddressesViewModel
        {
            Id = x.Id,
            Ip = x.Ip,
            CountryName = x.CountryName,
            City = x.City,
            CreatedDateTime = x.CreatedDateTime
        }));

        // GET: IpAddressesController/Details/5
        public IActionResult Details(int id)
        {
            var ipaddress = _ipAddressesData.GetIpAddressById(id);

            return ipaddress is null ? NotFound() : (IActionResult)View(ipaddress);
        }

        // GET: IpAddressesController/Create
        public IActionResult Create()
        {
            IpAddress ipAddress = _IpAddresses.LastOrDefault();
            return View(ipAddress);
        }

        // POST: IpAddressesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IpAddress IpAddress)
        {
            if (IpAddress is null)
                throw new ArgumentNullException(nameof(IpAddress));

            if (!ModelState.IsValid)
                return View(IpAddress);

            IpAddress.CreatedDateTime = DateTime.Now;
            var ipaddress = await _ipAddressesData.CreateIpAddressAsync(IpAddress);

            return RedirectToAction(nameof(Index));
        }

        // GET: IpAddressesController/Edit/5
        public IActionResult Edit(int? id)
        {
            if(id is null) return View(new IpAddress());

            if (id < 0) return BadRequest();

            var ipaddress = _ipAddressesData.GetIpAddressById((int)id);
            if (ipaddress is null) return NotFound();

            return View(ipaddress);
        }

        // POST: IpAddressesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IpAddress IpAddress)
        {
            if (IpAddress is null)
                throw new ArgumentNullException(nameof(IpAddress));

            if (!ModelState.IsValid) return View(IpAddress);

            //var id = IpAddress.Id;
            //if (id == 0)
            //    _ipAddressesData.Add(IpAddress);
            //else
            //    _ipAddressesData.Edit(id, IpAddress);

            //_ipAddressesData.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: IpAddressesController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var ipaddress = _ipAddressesData.GetIpAddressById(id);
            if (ipaddress is null)
                return NotFound();

            return View(ipaddress);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            _ipAddressesData.Delete(id);
            _ipAddressesData.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
