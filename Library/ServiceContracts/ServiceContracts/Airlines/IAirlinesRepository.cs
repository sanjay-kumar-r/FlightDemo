using AirlinesDTOs;
using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.Airlines
{
    public interface IAirlinesRepository
    {
        //Airlines --------------------------------------------------------------------------------
        IEnumerable<AirlinesDTOs.Airlines> GetAirlines(long? id = null);

        IEnumerable<AirlinesDTOs.Airlines> GetAirlinesbyIds(List<long> ids);

        IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition(AirlinesDTOs.AirlineDetails airline);

        IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByMultipleFilterconditions(List<AirlineDetails> airlineDetails);

        long AddAirline(AirlinesDTOs.Airlines airline);

        bool IsAirlineAlreadyExists(AirlinesDTOs.Airlines airline);

        bool IsAirlineIdsExists(List<long> ids);

        Result UpdateAirline(AirlinesDTOs.AirlineDetails airline, long userId);

        Result ActivateDeactivateAirline(AirlinesDTOs.Airlines airline);

        Result DeleteAirline(AirlinesDTOs.Airlines airline);

        Result PermanentDeleteAirline(long id);

        //DiscountTags --------------------------------------------------------------------------------
        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTags(long? id = null);

        IEnumerable<DiscountTags> GetDiscountTagByIds(List<long> ids = null);

        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTagsByFiltercondition(AirlinesDTOs.DiscountTagDetails discountTag);

        IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTagsByMultipleFilterconditions(List<AirlinesDTOs.DiscountTagDetails> discountTag);

        long AddDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        bool IsDiscountTagAlreadyExists(AirlinesDTOs.DiscountTags discountTag);

        Result UpdateDiscountTag(AirlinesDTOs.DiscountTagDetails discountTag, long userId);

        Result ActivateDeactivateDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        Result DeleteDiscountTag(AirlinesDTOs.DiscountTags discountTag);

        Result PermanentDeleteDiscountTag(long id);

        //Airline-DiscountTags Mappings --------------------------------------------------------------------------------
        bool AddAirlineDiscountTagMappings(List<AirlineDiscountTagMappings> airlineDiscountTagMappings);

        bool RemoveAirlineDiscountTagMappings(List<AirlineDiscountTagMappings> airlineDiscountTagMappings);

        IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappings(long? airlineId = null, long? discountId = null);

        IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappingsByIds(List<long> ids, bool isByAirlineId);

        //AirlineSchedules --------------------------------------------------------------------------------
        IEnumerable<AirlinesDTOs.AirlineSchedules> GetAirlineSchedules(long? id = null, bool isByAirlineId = false);

        IEnumerable<AirlineSchedules> GetAirlineSchedulesByIds(List<long> ids, bool isByAirlineId = false);

        IEnumerable<AirlinesDTOs.AirlineSchedules> GetGetAirlineSchedulesByFilterCondition(AirlinesDTOs.AirlineScheduleDetails schedule);

        IEnumerable<AirlineSchedules> GetGetAirlineSchedulesByMultipleFilterConditions(List<AirlineScheduleDetails> schedules);

        long AddAirlineSchedule(AirlinesDTOs.AirlineSchedules schedule);

        List<long> AddAirlineSchedulesByRange(List<AirlineSchedules> schedules);

        bool IsAirlineScheduleAlreadyExists(AirlineSchedules schedule);

        bool IsAirlineScheduleRangeAlreadyExists(List<AirlineSchedules> schedules);

        bool DeleteAirlineSchedule(long id, long userId);

        bool DeleteAirlineScheduleByScheduleIds(List<long> ids, long userId);

        Result PermanentDeleteAirlineSchedule(long id);

        //AirlineSchedulesTracker --------------------------------------------------------------------------------
        IEnumerable<AirlineScheduleTracker> GetAirlineScheduleTracker(long? id = null, bool isByScheduleId = false);

        IEnumerable<AirlineScheduleTracker> GetAirlineScheduleTrackerByIds(List<long> ids, bool isByScheduleId = false);

        IEnumerable<AirlineScheduleTracker> GetAirlineScheduleTrackerByFilterCondition(AirlinesDTOs.AirlineScheduleTracker scheduleTracker);

        long AddAirlineScheduleTracker(AirlineScheduleTracker scheduleTracker);

        bool UpdateAirlineScheduleTracker(long id, int bcTickets, int nbcTickets, bool isRevert = false);

        bool DeleteAirlineScheduleTracker(long id);

        bool DeleteAirlineScheduleTrackerByTrackerIds(List<long> ids);
    }
}
