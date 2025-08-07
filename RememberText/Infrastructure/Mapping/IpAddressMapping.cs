using RememberText.Domain.Entities;
using RememberText.Models;

namespace RememberText.Infrastructure.Mapping
{
    public static class IpAddressMapping
    {
        public static IpAddress ToDatabase(this IpInfo x) => new IpAddress
        {
            HostName = x.Hostname,
            City = x.City,
            Region = x.Region,
            CountryCode = x.Country,
            Loc = x.Loc,
            Organization = x.Org,
            Zip = x.Postal,
            Timezone = x.Timezone
        };
    }
}
