using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Airlines.Models.Utils
{
    public class DiscountTagsValidation
    {
        public static bool ValidateAddDiscountTag(AirlinesDTOs.DiscountTags discountTag)
        {
            if (discountTag == null || string.IsNullOrWhiteSpace(discountTag.Name) || discountTag.Discount <= 0)
                return false;
            return true;
        }

        public static bool ValidateUpdateDiscountTag(AirlinesDTOs.DiscountTagDetails discountTag)
        {
            if (discountTag == null || discountTag.Id <= 0 || discountTag.Discount <= 0)
                return false;
            return true;
        }

        public static bool ValidateActivateDeactivateDiscountTag(object id, object isActive)
        {
            if (id == null || isActive == null)
                return false;
            long id1;
            long.TryParse(id.ToString(), out id1);
            if (id1 <= 0 || !(new string[] { "true", "false" }).Contains(isActive.ToString().Trim(), StringComparer.OrdinalIgnoreCase))
                return false;
            return true;
        }

    }
}
