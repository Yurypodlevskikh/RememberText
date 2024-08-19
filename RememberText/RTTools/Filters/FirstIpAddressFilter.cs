using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RememberText.Controllers;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System;

namespace RememberText.RTTools.Filters
{
    public class FirstIpAddressFilter : Attribute, IResourceFilter
    {
        private readonly IIpAddressesData _ipAddressesData;
        private readonly IMemoryCache _cache;
        public FirstIpAddressFilter(IIpAddressesData ipAddressesData, IMemoryCache cache)
        {
            _ipAddressesData = ipAddressesData;
            _cache = cache;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string ipaddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipaddress == "::1")
            {
                ipaddress = "83.248.115.115";
                //ipaddress = "134.201.250.155";
            }

            if (!string.IsNullOrEmpty(ipaddress))
            {
                if (string.IsNullOrEmpty(_cache.Get<string>(ipaddress)))
                {
                    var ipaddressInDb = _ipAddressesData.GetIpAddressByIpAddressAsync(ipaddress).Result;

                    MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                    cacheOptions.SlidingExpiration = TimeSpan.FromMinutes(1);

                    if (ipaddressInDb == null)
                    {
                        IpAddress ipAddress = new IpAddress
                        {
                            Ip = ipaddress,
                            CreatedDateTime = DateTime.Now
                        };

                        IpAddress ipAddressResponse = _ipAddressesData.CreateIpAddress(ipAddress);

                        if (ipAddressResponse != null)
                        {
                            _cache.Set<string>(ipaddress, ipAddressResponse.Id.ToString(), cacheOptions);
                        }

                        context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                        {
                            {"controller","Home" },
                            {"action", "Introduction" }
                        });
                    }
                    else
                    {
                        _cache.Set<string>(ipaddress, ipaddressInDb.Id.ToString(), cacheOptions);
                    }
                }
            }
        }
    }
}
