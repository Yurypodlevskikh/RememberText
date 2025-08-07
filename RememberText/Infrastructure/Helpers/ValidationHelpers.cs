using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace RememberText.Infrastructure.Helpers
{
    public static class ValidationHelpers
    {
        public static void GetLocalReturnUrl(this HttpRequest req, out string returnUrl)
        {
            var referer = req.Headers["Referer"];
            Uri refererUri = new Uri(referer);
            string thisWebsiteHost = refererUri.IsLoopback ? "localhost" : "remembertext.eu";
            string host = refererUri.Host;

            bool isLocal = string.IsNullOrEmpty(referer) ? false :
                host.Equals(thisWebsiteHost) ? true : false;

            returnUrl = isLocal ? refererUri.PathAndQuery : "/";
        }

        public static void GetLocalOrSessionReturnUrl(this HttpRequest req, string[] compareUrl, out string returnUrl)
        {
            req.GetLocalReturnUrl(out returnUrl);

            if (compareUrl != null && compareUrl.Length > 1)
            {
                string[] urlArr = returnUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (urlArr.Length > 1 && (urlArr[0] == compareUrl[0] && urlArr[1] == compareUrl[1]))
                {
                    string sessionString = req.HttpContext.Session.GetString("ReturnUrl");
                    if (sessionString != null || !string.IsNullOrEmpty(sessionString))
                    {
                        returnUrl = sessionString;
                    }
                }
                else
                {
                    req.HttpContext.Session.SetString("ReturnUrl", returnUrl);
                }
            }
        }

        public static bool IsAjaxRequest(this HttpRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException("request");
            }

            if (req.Headers != null)
            {
                return req.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }
    }
}
