using CommonDTOs;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Airlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Airlines.Models.Utils
{
    public class AirlinesRepository : IAirlinesRepository
    {
        private readonly AirlinesDBContext context;

        public AirlinesRepository(AirlinesDBContext context)
        {
            this.context = context;
        }

        public Result ActivateDeactivateAirline(AirlinesDTOs.Airlines airline)
        {
            Result result = new Result();
            if (airline.Id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == airline.Id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.Airlines airlineExisting = context.Airlines.First(x => x.Id == airline.Id);
                airlineExisting.IsActive = airline.IsActive;
                airlineExisting.ModifiedOn = DateTime.Now;
                airlineExisting.ModifiedBy = airline.ModifiedBy;
                //airlineExisting.CreatedUser = null;
                //airlineExisting.ModifiedUser = null;

                var airlineUpdated = context.Airlines.Attach(airlineExisting);
                airlineUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated airline as '{airline.IsActive}' for id={airline.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update airline as '{airline.IsActive}' for id={airline.Id} \n " +
                    "Invalid_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        public long AddAirline(AirlinesDTOs.Airlines airline)
        {
            airline.CreatedOn = DateTime.Now;
            airline.ModifiedOn = DateTime.Now;
            //airline.CreatedUser = null;
            //airline.ModifiedUser = null;
            context.Airlines.Add(airline);
            context.SaveChanges();
            long id = airline.Id;
            return id;
        }

        public bool FlightExists(AirlinesDTOs.Airlines airline)
        {
            return context.Airlines.Any(x => (string.IsNullOrWhiteSpace(airline.Name) || x.Name.Equals(airline.Name))
                && (string.IsNullOrWhiteSpace(airline.AirlineCode) || x.AirlineCode.Equals(airline.AirlineCode))
                && (airline.Id <= 0 || airline.Id != airline.Id));
        }

        public Result DeleteAirline(AirlinesDTOs.Airlines airline)
        {
            Result result = new Result();
            if (airline.Id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == airline.Id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.Airlines airlineExisting = context.Airlines.First(x => x.Id == airline.Id);
                airlineExisting.IsDeleted = true;
                airlineExisting.ModifiedOn = DateTime.Now;
                airlineExisting.ModifiedBy = airline.ModifiedBy;
                //airlineExisting.CreatedUser = null;
                //airlineExisting.ModifiedUser = null;

                var airlineUpdated = context.Airlines.Attach(airlineExisting);
                airlineUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated airline as deleted for id={airline.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update airline as deleted for id={airline.Id} \n " + 
                    "Invalid_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlines(long? id = null)
        {
            if (id == null)
                return context.Airlines.ToList();
            else
                return new List<AirlinesDTOs.Airlines>() { context.Airlines.FirstOrDefault(x => x.Id == id) };
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition(AirlinesDTOs.Airlines airline)
        {
            if (airline == null || (airline.Id <= 0 && string.IsNullOrWhiteSpace(airline.Name) && string.IsNullOrWhiteSpace(airline.AirlineCode)
                    && airline.BCTicketCost <= 0 && airline.NBCTicketCost <= 0))
                return context.Airlines.ToList();
            return context.Airlines.Where(x => (airline.Id <= 0 || airline.Id == x.Id)
                && (string.IsNullOrWhiteSpace(airline.Name) || x.Name.Contains(airline.Name))
                && (string.IsNullOrWhiteSpace(airline.AirlineCode) || x.AirlineCode.Contains(airline.AirlineCode))
                && (airline.BCTicketCost <= 0 || x.BCTicketCost <= airline.BCTicketCost)
                && (airline.NBCTicketCost <= 0 || x.NBCTicketCost <= airline.NBCTicketCost)
                && !x.IsDeleted
                );
        }

        public Result PermanentDelete(long id)
        {
            Result result = new Result();
            if (id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == id))
            {
                AirlinesDTOs.Airlines airline = new AirlinesDTOs.Airlines() { Id = id };
                context.Airlines.Remove(airline);
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully deleted(permanent) airline date for id={id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to delete(permanent) airline date for id={id} " + Environment.NewLine +
                    "Invalid_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        public Result UpdateAirline(AirlinesDTOs.Airlines airline)
        {
            Result result = new Result();
            if (airline.Id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == airline.Id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.Airlines airlineExisting = context.Airlines.First(x => x.Id == airline.Id);
                if (!string.IsNullOrWhiteSpace(airline.Name))
                    airlineExisting.Name = airline.Name;
                if (!string.IsNullOrWhiteSpace(airline.AirlineCode))
                    airlineExisting.AirlineCode = airline.AirlineCode;
                if (!string.IsNullOrWhiteSpace(airline.ContactNumber))
                    airlineExisting.ContactNumber = airline.ContactNumber;
                if (!string.IsNullOrWhiteSpace(airline.ContactAddress))
                    airlineExisting.ContactAddress = airline.ContactAddress;
                if (airline.TotalSeats > 0)
                    airlineExisting.TotalSeats = airline.TotalSeats;
                if (airline.TotalSeats > 0)
                    airlineExisting.TotalSeats = airline.TotalSeats;
                airlineExisting.ModifiedOn = DateTime.Now;
                airlineExisting.ModifiedBy = airline.ModifiedBy;
                //airlineExisting.CreatedUser = null;
                //airlineExisting.ModifiedUser = null;

                var airlineUpdated = context.Airlines.Attach(airlineExisting);
                airlineUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated airline date for id={airline.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update airline date for id={airline.Id} \n " +
                    "Invalid_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }
    }
}
