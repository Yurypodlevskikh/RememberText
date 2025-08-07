using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using RememberText.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RememberText.RTTools.Filters
{
    public class FirstIpAddressFilter : Attribute, IAsyncResourceFilter
    {
        private readonly IIpAddressesData _ipAddressesData;
        private readonly SignInManager<User> _signInManager;
        private readonly IRTVisitorInfoData _visitorService;
        private readonly IMemoryCache _cache;
        public FirstIpAddressFilter(IIpAddressesData ipAddressesData,
            IMemoryCache cache,
            IRTVisitorInfoData visitorService,
            SignInManager<User> signInManager)
        {
            _ipAddressesData = ipAddressesData;
            _visitorService = visitorService;
            _signInManager = signInManager;
            _cache = cache;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            ClaimsPrincipal user = context.HttpContext.User;
            // If the User isn't signed in
            if (!_signInManager.IsSignedIn(user))
            {
                string ipaddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                // If an incomming ip address is local (developing) change it to my swedish ip address as a test ip
                if (ipaddress == "::1")
                {
                    ipaddress = "83.248.115.115";
                    //ipaddress = "134.201.250.155";
                }

                if (!string.IsNullOrEmpty(ipaddress))
                {
                    var consentFeature = context.HttpContext.Features.Get<ITrackingConsentFeature>();
                    bool canCreateCookies = consentFeature?.CanTrack ?? false;
                    bool ipExists = true;

                    if (canCreateCookies)
                    {
                        if (!context.HttpContext.Request.Cookies.ContainsKey(ipaddress))
                        {
                            // Create a cookie
                            context.HttpContext.Response.Cookies.Append(ipaddress, ipaddress);

                            // Checking if the ip address saved in the database
                            ipExists = await _ipAddressesData.IpAddressExistsAsync(ipaddress);
                        }
                    }
                    else
                    {
                        // If the ip don't exists in the cache, then save or create and save the ip address
                        if (string.IsNullOrEmpty(_cache.Get<string>(ipaddress)))
                        {
                            // Create options for cache memory
                            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                            cacheOptions.SlidingExpiration = TimeSpan.FromDays(1);
                            // Save the new ip address in the cach memory
                            _cache.Set(ipaddress, ipaddress, cacheOptions);

                            // Checking if the ip address saved in the database
                            ipExists = await _ipAddressesData.IpAddressExistsAsync(ipaddress);
                        }
                    }

                    if(!ipExists)
                    {
                        IpAddress ipAddress = new IpAddress();

                        // Get ip informataion by webpage ipinfo.io
                        IpInfo ipInfo = _visitorService.GetIpInfoByIpAddress(ipaddress);
                        ipAddress = ipInfo.ToDatabase();

                        string userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
                        ipAddress.Ip = ipaddress;
                        ipAddress.CreatedDateTime = DateTime.Now;
                        ipAddress.Browser = _visitorService.GetOSBrowserInfo(userAgent);

                        IpAddress ipAddressNew = _ipAddressesData.CreateIpAddress(ipAddress);
                        if (ipAddressNew != null)
                        {
                            // Save visiting on the database
                            //await _visitService.SaveVisiting(ipAddressNew.Id);
                        }

                        // Send it to the Introduction page
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                            {
                                {"controller","Home" },
                                {"action", "Introduction" }
                            });

                        return;
                    }
                }
                else
                {
                    // Send it to the Introduction page
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                            {
                                {"controller","Home" },
                                {"action", "Introduction" }
                            });

                    return;
                }
            }

            await next();
        }
    }
}
