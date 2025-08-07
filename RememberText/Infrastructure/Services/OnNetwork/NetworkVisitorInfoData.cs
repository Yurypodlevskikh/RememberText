using Newtonsoft.Json;
using RememberText.Infrastructure.Interfaces;
using RememberText.Models;
using System.Net;
using UAParser;

namespace RememberText.Infrastructure.Services.OnNetwork
{
    public class NetworkVisitorInfoData : IRTVisitorInfoData
    {
        public IpInfo GetIpInfoByIpAddress(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            string ipRequest = "https://ipinfo.io/" + ip + "?token=60f42012b8da8b";
            string info = new WebClient().DownloadString(ipRequest);
            ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            return ipInfo;
        }

        public string GetOSBrowserInfo(string userAgent)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            string os = c.OS.ToString(); // Operative system
            string ua = c.UA.ToString(); // Browser
            return string.IsNullOrEmpty(os) ? ua : os + " " + ua;
        }
    }
}
