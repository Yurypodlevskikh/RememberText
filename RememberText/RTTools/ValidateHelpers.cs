using Microsoft.AspNetCore.Http;
using RememberText.Models;
using System;
using System.Net;
//using System.Security.Policy;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RememberText.RTTools
{
    public static class ValidateHelpers
    {
        #region EmailIsValid
        static Regex ValidEmailRegex = CreateValidEmailRegex();
        
        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        internal static bool EmailIsValid(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);

            return isValid;
        }
        #endregion EmailIsValid

        #region DecimalIsValid
        internal static DecimalIsValidResponse DecimalIsValid(string amount)
        {
            bool comma = amount.Contains(",");
            bool dot = amount.Contains(".");
            bool commaDot = comma == dot ? true : false;
            string validDecimalPattern = "";

            DecimalIsValidResponse response = new DecimalIsValidResponse();
            response.IsValid = false;
            response.DecimalString = amount;
            
            // These can't be the same value
            if (!commaDot)
            {
                if (comma)
                {
                    validDecimalPattern = @"\d+(\,\d{1,2})?";
                }
                else if (dot)
                {
                    validDecimalPattern = @"\d+(\.\d{1,2})?";
                    
                }

                Regex ValidDecimalRegex = new Regex(validDecimalPattern);
                if (ValidDecimalRegex.IsMatch(amount))
                {
                    response.IsValid = true;
                    if (comma)
                    {   
                        response.DecimalString = amount.Replace(",", ".");
                    }
                }
            }
            else
            {
                Regex ValidIntegerRegex = new Regex(@"^\d+$");
                if(ValidIntegerRegex.IsMatch(amount))
                {
                    response.IsValid = true;
                }
            }
            
            return response;
        }
        #endregion DecimalIsValid

        #region IPAddress
        public static string getIPAddress(IHttpContextAccessor httpContextAccessor)
        {
            const string myIp = "83.248.115.115";
            string ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            //var ipData = WebClient()

            return ip;
        }

        private const string NullIpAddress = "::1";
        public static void IsLocalReturnUrl(HttpRequest req, out string returnUrl)
        {
            //bool isIpAddress = IPAddress.TryParse("134.201.250.155", out remoteIpAddress);

            ConnectionInfo connection = req.HttpContext.Connection;
            IPAddress remoteIpAddress = connection.RemoteIpAddress;
            IPAddress localIpAddress = connection.LocalIpAddress;
            returnUrl = "~/";

            bool isLocal = remoteIpAddress.IsSet() ? localIpAddress.IsSet() ? remoteIpAddress.Equals(localIpAddress) : IPAddress.IsLoopback(remoteIpAddress) : true;

            if(isLocal)
            {
                Uri uri = new Uri(req.Headers["Referer"]);
                returnUrl = uri.PathAndQuery;
            }
        }

        private static bool IsSet(this IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }
        #endregion IPAddress

        public static bool IsAjaxRequest(this HttpRequest req)
        {
            if(req == null)
            {
                throw new ArgumentNullException("request");
            }

            if(req.Headers != null)
            {
                return req.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }
    }
}