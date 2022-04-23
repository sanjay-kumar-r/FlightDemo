using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Airlines.Models.Utils
{
    public class AirlinesValidation
    {
        //Airlines --------------------------------------------------------------------------------
        public static bool ValidateAddFlight(AirlinesDTOs.Airlines airline)
        {
            if (airline == null || string.IsNullOrWhiteSpace(airline.Name) || string.IsNullOrWhiteSpace(airline.ContactNumber)
                || airline.TotalSeats <= 0 || airline.TotalBCSeats < 0 || airline.TotalNBCSeats < 0 || 
                (airline.TotalBCSeats + airline.TotalNBCSeats) != airline.TotalSeats ||
                airline.BCTicketCost < (double)0 || airline.NBCTicketCost < (double)0)
                return false;
            return true;
        }

        public static bool ValidateUpdateFlight(AirlinesDTOs.AirlineDetails airline)
        {
            if (airline == null || airline.Id <= 0
                || airline.TotalSeats <= 0 || airline.TotalBCSeats < 0 || airline.TotalNBCSeats < 0 ||
                (airline.TotalBCSeats + airline.TotalNBCSeats) != airline.TotalSeats ||
                airline.BCTicketCost < (double)0 || airline.NBCTicketCost < (double)0)
                return false;
            return true;
        }

        public static bool ValidateActivateDeactivateAirline(object id, object isActive)
        {
            if (id == null || isActive == null)
                return false;
            long id1;
            long.TryParse(id.ToString(), out id1);
            if(id1 <= 0 || !(new string[] { "true","false"}).Contains(isActive.ToString().Trim(), StringComparer.OrdinalIgnoreCase))
                return false;
            return true;
        }

        //DiscountTags --------------------------------------------------------------------------------
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

        //Airline-DiscountTags Mappings --------------------------------------------------------------------------------
        public static bool ValidateAirlineDiscountTagMappings(List<AirlinesDTOs.AirlineDiscountTagMappingDetails> mappings)
        {
            if (mappings == null || mappings.Count() <= 0)
                return false;
            return true;
        }

        public static bool ValidateRemapAirlineDiscountTagsDetails(List<AirlinesDTOs.RemapAirlineDiscountTagsDetails> mappings)
        {
            if (mappings == null || mappings.Count() <= 0)
                return false;
            return true;
        }

        //AirlineSchedules --------------------------------------------------------------------------------
        public static bool ValidateAddAirlineSchedule(AirlinesDTOs.AirlineSchedules schedule)
        {
            if (schedule == null || schedule.AirlineId <= 0 || string.IsNullOrWhiteSpace(schedule.From) || string.IsNullOrWhiteSpace(schedule.To)
                || (schedule.IsRegular && (schedule.DepartureDay == null || !Enum.IsDefined(typeof(DayOfWeek),schedule.DepartureDay))
                    && (schedule.ArrivalDay == null || !Enum.IsDefined(typeof(DayOfWeek), schedule.ArrivalDay)) )
                || (!schedule.IsRegular && (schedule.DepartureDate == null || schedule.ArrivalDate == null))
                || schedule.DepartureTime == null || schedule.ArrivalTime == null)
                return false;
            return true;
        }

        public static bool ValidateAddAirlineScheduleByRangs(List<AirlinesDTOs.AirlineSchedules> schedules)
        {
            if (schedules == null || schedules.Count() <= 0)
                return false;
            foreach(var schedule in schedules)
            {
                if (!ValidateAddAirlineSchedule(schedule))
                    return false;
            }
            return true;
        }

    }
}
