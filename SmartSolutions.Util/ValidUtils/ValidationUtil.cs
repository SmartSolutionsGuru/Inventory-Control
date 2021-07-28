using System.Text.RegularExpressions;

namespace SmartSolutions.Util.ValidUtils
{
    public static class ValidationUtil
    {
        public static bool IsValidName(string name)
        {
            bool retVal = false;
            var nameVerificationRegex = @"^[A-Z][a-zA-Z]*$";
            if (Regex.Match(name, nameVerificationRegex).Success)
            {
                retVal = true;
            }
            return retVal;
        }
        public static bool IsValidMobileNumber(string mobile)
        {
            bool retVal = false;
            var mobileVerificationRegex = @"^((\(((\+|00)92)\)|(\+|00)92)(( |\-)?)(3[0-9]{2})\6|0(3[0-9]{2})( |\-)?)[0-9]{3}( |\-)?[0-9]{4}$";
            if (Regex.Match(mobile, mobileVerificationRegex).Success)
            {
                retVal = true;
            }
            return retVal;
        }
        public static bool IsValidPhoneNumber(string phone)
        {
            bool retVal = false;
            var landLineRegex = @"^(\((\+|00)92\)( )?|(\+|00)92( )?|0)[1-24-9]([0-9]{1}( )?[0-9]{3}( )?[0-9]{3}( )?[0-9]{1,2}|[0-9]{2}( )?[0-9]{3}( )?[0-9]{3})$";
            if (Regex.Match(phone, landLineRegex).Success)
            {
                retVal = true;
            }
            return retVal;
        }
        public static bool IsValidPremiumNumber(string premiumNumber)
        {
            bool retVal = false;
            var regexForPremiumNumber = @"^0(8|9)00 ?[0-9]{3} ?[0-9]{2}$";
            if (Regex.Match(premiumNumber, regexForPremiumNumber).Success)
            {
                retVal = true;
            }
            return retVal;
        }
    }
}
