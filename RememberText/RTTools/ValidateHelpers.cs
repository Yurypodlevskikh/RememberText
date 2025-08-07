using RememberText.Models;
//using System.Security.Policy;
using System.Text.RegularExpressions;

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
    }
}