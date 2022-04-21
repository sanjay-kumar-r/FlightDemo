using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.Airlines
{
    public interface IDiscountTagsRepository
    {
        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTags(long? id = null);

        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTagsByFiltercondition(AirlinesDTOs.DiscountTagDetails discountTag);

        long AddDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        bool DiscountTagExists(AirlinesDTOs.DiscountTags discountTag);

        Result UpdateDiscountTag(AirlinesDTOs.DiscountTagDetails discountTag, long userId);

        Result ActivateDeactivateDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        Result DeleteDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        Result PermanentDelete(long id);
    }
}
