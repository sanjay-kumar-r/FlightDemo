using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Airlines.Models.Utils
{
    public class AirlinesValidation
    {
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

        public static bool ValidateAirlineDiscountTagMappings(List<AirlinesDTOs.AirlineDiscountTagMappingDetails> mappings)
        {
            if (mappings == null || mappings.Count() <= 0)
                return false;
            return true;
        }

    }
}
