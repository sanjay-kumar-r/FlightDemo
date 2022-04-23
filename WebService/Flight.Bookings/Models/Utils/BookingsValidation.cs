using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flight.Users.Model.Utils
{
    public class BookingsValidation
    {
        //public static bool ValidateRegistration(UserDtOs.Users user)
        //{
        //    if (string.IsNullOrWhiteSpace(user.FirstName) ||
        //        string.IsNullOrWhiteSpace(user.EmailId) || string.IsNullOrWhiteSpace(user.Password))
        //        return false;
        //    if (!string.IsNullOrWhiteSpace(user.EmailId))
        //    {
        //        var regex = "^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$";
        //        var match = Regex.Match(user.EmailId, regex, RegexOptions.IgnoreCase);
        //        if (!match.Success)
        //            return false;
        //    }
        //    return true;
                
        //}

        //public static bool ValidateLogin(UserDtOs.Users user)
        //{
        //    if (string.IsNullOrWhiteSpace(user.EmailId) || string.IsNullOrWhiteSpace(user.Password))
        //        return false;
        //    return true;

        //}

        //public static bool ValidateUpdate(UserDtOs.Users user)
        //{
        //    if (user.Id <= 0)
        //        return false;
        //    if (!string.IsNullOrWhiteSpace(user.EmailId))
        //    {
        //        var regex = "^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$";
        //        var match = Regex.Match(user.EmailId, regex, RegexOptions.IgnoreCase);
        //        if (!match.Success)
        //            return false;
        //    }
        //    return true;

        //}
    }
}
