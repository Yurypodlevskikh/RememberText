using RememberText.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTVisitorInfoData
    {
        IpInfo GetIpInfoByIpAddress(string ip);
        string GetOSBrowserInfo(string userAgent);
    }
}
