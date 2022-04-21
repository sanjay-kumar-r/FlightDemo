using AirlinesDTOs;
using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.Airlines
{
    public interface IAirlinesRepository
    {
        //Airlines
        IEnumerable<AirlinesDTOs.Airlines> GetAirlines(long? id = null);

        IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition(AirlinesDTOs.AirlineDetails airline);

        IEnumerable<DiscountTags> GetAirlinesByMultipleFilterconditions(List<AirlineDetails> airlineDetails);

        long AddAirline(AirlinesDTOs.Airlines airline);

        bool IsAirlineAlreadyExists(AirlinesDTOs.Airlines airline);

        Result UpdateAirline(AirlinesDTOs.AirlineDetails airline, long userId);

        Result ActivateDeactivateAirline(AirlinesDTOs.Airlines airline);

        Result DeleteAirline(AirlinesDTOs.Airlines airline);

        Result PermanentDeleteAirline(long id);

        //DiscountTags
        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTags(long? id = null);

        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTagsByFiltercondition(AirlinesDTOs.DiscountTagDetails discountTag);

        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTagsByMultipleFilterconditions(List<AirlinesDTOs.DiscountTagDetails> discountTag);

        long AddDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        bool IsDiscountTagAlreadyExists(AirlinesDTOs.DiscountTags discountTag);

        Result UpdateDiscountTag(AirlinesDTOs.DiscountTagDetails discountTag, long userId);

        Result ActivateDeactivateDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        Result DeleteDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        Result PermanentDeleteDiscountTag(long id);

        //AirlineDiscountTagMappings
        Result AddAirlineDiscountTagMappings(List<AirlineDiscountTagMappings> airlineDiscountTagMappings);

        IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappings(long? airlineId = null, long? discountId = null);
    }
}
