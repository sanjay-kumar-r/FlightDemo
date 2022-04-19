using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.Airlines
{
    public interface IAirlinesRepository
    {
        IEnumerable<AirlinesDTOs.Airlines> GetAirlines(long? id = null);

        IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition(AirlinesDTOs.Airlines airline);

        long AddAirline(AirlinesDTOs.Airlines airline);

        bool FlightExists(AirlinesDTOs.Airlines airline);

        Result UpdateAirline(AirlinesDTOs.Airlines airline);

        Result ActivateDeactivateAirline(AirlinesDTOs.Airlines airline);

        Result DeleteAirline(AirlinesDTOs.Airlines airline);

        Result PermanentDelete(long id);
    }
}
