using AirlinesDTOs;
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
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //Airlines --------------------------------------------------------------------------------
        public Result ActivateDeactivateAirline(AirlinesDTOs.Airlines airline)
        {
            Result result = new Result();
            if (airline.Id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == airline.Id && !x.IsDeleted))
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
                result.ResultMessage = $"unable to update airline as '{airline.IsActive}' for id={airline.Id} " + Environment.NewLine +
                    "Invalid/Deleted_AirlineId_or_No_data_exists_that_matches_AirlineId";
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

        public bool IsAirlineAlreadyExists(AirlinesDTOs.Airlines airline)
        {
            return context.Airlines.Any(x => (string.IsNullOrWhiteSpace(airline.Name) || x.Name.Equals(airline.Name))
                && (string.IsNullOrWhiteSpace(airline.AirlineCode) || x.AirlineCode.Equals(airline.AirlineCode))
                && (airline.Id <= 0 || airline.Id != x.Id) && !x.IsDeleted);
        }

        public bool IsAirlineIdsExists(List<long> ids)
        {
            return context.Airlines.AsEnumerable().Where(x => ids.Contains(x.Id) && !x.IsDeleted).Any();
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
                result.ResultMessage = $"unable to update airline as deleted for id={airline.Id} " + Environment.NewLine +
                    "Invalid_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlines(long? id = null)
        {
            if (id == null)
                return context.Airlines.Where(x => !x.IsDeleted);
            else
                return new List<AirlinesDTOs.Airlines>() { context.Airlines.FirstOrDefault(x => x.Id == id && !x.IsDeleted) };
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesbyIds(List<long> ids)
        {
            if (ids == null || ids.Count() <= 0)
                return context.Airlines.Where(x => !x.IsDeleted);
            else
                return context.Airlines.AsEnumerable().Where(x => ids.Contains(x.Id) && !x.IsDeleted);
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition(AirlinesDTOs.AirlineDetails airline)
        {
            if (airline == null || (airline.Id <= 0 && string.IsNullOrWhiteSpace(airline.Name) && string.IsNullOrWhiteSpace(airline.AirlineCode)
                    && airline.BCTicketCost <= 0 && airline.NBCTicketCost <= 0 && airline.IsActive == null))
                return context.Airlines.Where(x => !x.IsDeleted);
            return context.Airlines.Where(x => (airline.Id <= 0 || airline.Id == x.Id)
                && (string.IsNullOrWhiteSpace(airline.Name) || x.Name.Contains(airline.Name))
                && (string.IsNullOrWhiteSpace(airline.AirlineCode) || x.AirlineCode.Contains(airline.AirlineCode))
                && (airline.BCTicketCost <= 0 || x.BCTicketCost <= airline.BCTicketCost)
                && (airline.NBCTicketCost <= 0 || x.NBCTicketCost <= airline.NBCTicketCost)
                && (airline.IsActive == null || x.IsActive == (bool)airline.IsActive)
                && !x.IsDeleted
                );
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByMultipleFilterconditions(List<AirlineDetails> airlineDetails)
        {
            if (airlineDetails == null || airlineDetails.Count() <= 0)
                return context.Airlines.Where(x => !x.IsDeleted);
            Guid g = Guid.NewGuid();
            string splitter = g.ToString();
            g = Guid.NewGuid();
            string idPlaceHolder = g.ToString();
            g = Guid.NewGuid();
            string namePlaceHolder = g.ToString();
            g = Guid.NewGuid();
            string codePlaceHolder = g.ToString();
            var airlinesGrouped = airlineDetails.Select(x => (x.Id > 0 ? x.Id.ToString() : idPlaceHolder) + splitter +
                (!string.IsNullOrWhiteSpace(x.Name) ? x.Name : namePlaceHolder) + splitter +
                (!string.IsNullOrWhiteSpace(x.AirlineCode) ? x.AirlineCode : codePlaceHolder));
            return context.Airlines.AsEnumerable().Where(x => !x.IsDeleted &&
                airlinesGrouped.Any(y =>
                y.Replace(idPlaceHolder, x.Id.ToString()).Replace(namePlaceHolder, x.Name).Replace(codePlaceHolder, x.AirlineCode)
                .Equals(x.Id + splitter + x.Name + splitter + x.AirlineCode)));
        }

        public Result PermanentDeleteAirline(long id)
        {
            Result result = new Result();
            if (id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == id))
            {
                AirlinesDTOs.Airlines airline = new AirlinesDTOs.Airlines() { Id = id };
                context.Airlines.Remove(airline);
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully deleted(permanent) airline data for id={id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to delete(permanent) airline data for id={id} " + Environment.NewLine +
                    "Invalid_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        public Result UpdateAirline(AirlinesDTOs.AirlineDetails airline, long userId)
        {
            Result result = new Result();
            if (airline.Id > 0 && context.Airlines.Count() > 0 && context.Airlines.Any(x => x.Id == airline.Id && !x.IsDeleted))
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
                if (airline.TotalBCSeats > 0)
                    airlineExisting.TotalBCSeats = airline.TotalBCSeats;
                if (airline.TotalNBCSeats > 0)
                    airlineExisting.TotalBCSeats = airline.TotalNBCSeats;
                if(airline.BCTicketCost > 0)
                    airlineExisting.BCTicketCost = airline.BCTicketCost;
                if (airline.NBCTicketCost > 0)
                    airlineExisting.NBCTicketCost = airline.NBCTicketCost;
                if (airline.IsActive != null)
                    airlineExisting.IsActive = (bool)airline.IsActive;
                airlineExisting.ModifiedOn = DateTime.Now;
                airlineExisting.ModifiedBy = userId;
                //airlineExisting.CreatedUser = null;
                //airlineExisting.ModifiedUser = null;

                var airlineUpdated = context.Airlines.Attach(airlineExisting);
                airlineUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated airline data for id={airline.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update airline data for id={airline.Id} " + Environment.NewLine +
                    "Invalid/Deleted_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        //DiscountTags --------------------------------------------------------------------------------
        public IEnumerable<DiscountTags> GetDiscountTags(long? id = null)
        {
            if (id == null)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            else
                return new List<AirlinesDTOs.DiscountTags>() { context.DiscountTags.FirstOrDefault(x => x.Id == id && !x.IsDeleted) };
        }

        public IEnumerable<DiscountTags> GetDiscountTagByIds(List<long> ids = null)
        {
            if (ids == null)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            else
                return context.DiscountTags.Where(x => !x.IsDeleted && ids.Contains(x.Id));
        }

        public IEnumerable<DiscountTags> GetDiscountTagsByFiltercondition(DiscountTagDetails discountTag)
        {
            if (discountTag == null || (discountTag.Id <= 0 && string.IsNullOrWhiteSpace(discountTag.Name) && string.IsNullOrWhiteSpace(discountTag.DiscountCode)
                    && discountTag.Discount <= 0) || discountTag.IsActive == null)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            return context.DiscountTags.Where(x => (discountTag.Id <= 0 || discountTag.Id == x.Id) && !x.IsDeleted
                && (string.IsNullOrWhiteSpace(discountTag.Name) || x.Name.Contains(discountTag.Name))
                && (string.IsNullOrWhiteSpace(discountTag.DiscountCode) || x.DiscountCode.Contains(discountTag.DiscountCode))
                && (discountTag.Discount <= 0 || x.Discount <= discountTag.Discount)
                && (discountTag.IsActive == null || x.IsActive == discountTag.IsActive)
                );
        }

        public IEnumerable<DiscountTags> GetDiscountTagsByMultipleFilterconditions(List<DiscountTagDetails> discountTags)
        {
            if (discountTags == null || discountTags.Count() <= 0)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            Guid g = Guid.NewGuid();
            string splitter = g.ToString();
            g = Guid.NewGuid();
            string idPlaceHolder = g.ToString();
            g = Guid.NewGuid();
            string namePlaceHolder = g.ToString();
            g = Guid.NewGuid();
            string codePlaceHolder = g.ToString();
            var discountTagsGrouped = discountTags.Select(x => (x.Id > 0 ? x.Id.ToString() : idPlaceHolder) + splitter +
                (!string.IsNullOrWhiteSpace(x.Name) ? x.Name : namePlaceHolder) + splitter +
                (!string.IsNullOrWhiteSpace(x.DiscountCode) ? x.DiscountCode : codePlaceHolder));
            return context.DiscountTags.AsEnumerable().Where(x => !x.IsDeleted &&
                discountTagsGrouped.Any(y => 
                y.Replace(idPlaceHolder, x.Id.ToString()).Replace(namePlaceHolder, x.Name).Replace(codePlaceHolder, x.DiscountCode)
                .Equals(x.Id + splitter + x.Name + splitter + x.DiscountCode)));
        }

        public long AddDiscountTag(DiscountTags discountTag)
        {
            discountTag.CreatedOn = DateTime.Now;
            discountTag.ModifiedOn = DateTime.Now;
            //discountTag.CreatedUser = null;
            //discountTag.ModifiedUser = null;
            context.DiscountTags.Add(discountTag);
            context.SaveChanges();
            long id = discountTag.Id;
            return id;
        }

        public bool IsDiscountTagAlreadyExists(DiscountTags discountTag)
        {
            return context.DiscountTags.Any(x => (string.IsNullOrWhiteSpace(discountTag.Name) || x.Name.Equals(discountTag.Name))
                && (string.IsNullOrWhiteSpace(discountTag.DiscountCode) || x.DiscountCode.Equals(discountTag.DiscountCode))
                && (discountTag.Id <= 0 || discountTag.Id != discountTag.Id) && !x.IsDeleted);
        }

        public Result UpdateDiscountTag(DiscountTagDetails discountTag, long userId)
        {
            Result result = new Result();
            if (discountTag.Id > 0 && context.DiscountTags.Count() > 0 && context.DiscountTags.Any(x => x.Id == discountTag.Id && !x.IsDeleted))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.DiscountTags discountTagExisting = context.DiscountTags.First(x => x.Id == discountTag.Id);
                if (!string.IsNullOrWhiteSpace(discountTag.Name))
                    discountTagExisting.Name = discountTag.Name;
                if (!string.IsNullOrWhiteSpace(discountTag.DiscountCode))
                    discountTagExisting.DiscountCode = discountTag.DiscountCode;
                if (!string.IsNullOrWhiteSpace(discountTag.Description))
                    discountTagExisting.Description = discountTag.Description;
                if (discountTag.Discount > 0)
                    discountTagExisting.Discount = discountTag.Discount;
                if (discountTag.IsByRate != null)
                    discountTagExisting.IsByRate = (bool)discountTag.IsByRate;
                if (discountTag.IsActive != null)
                    discountTagExisting.IsActive = (bool)discountTag.IsActive;
                discountTagExisting.ModifiedOn = DateTime.Now;
                discountTagExisting.ModifiedBy = userId;
                //discountTagExisting.CreatedUser = null;
                //airldiscountTagExistingineExisting.ModifiedUser = null;

                var discountTagUpdated = context.DiscountTags.Attach(discountTagExisting);
                discountTagUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated discountTag data for id={discountTag.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update discountTag data for id={discountTag.Id} " + Environment.NewLine +
                    "Invalid/Deleted_discountTagId_or_No_data_exists_that_matches_discountTagId";
            }
            return result;
        }

        public Result ActivateDeactivateDiscountTag(DiscountTags discountTag)
        {
            Result result = new Result();
            if (discountTag.Id > 0 && context.DiscountTags.Count() > 0 && context.DiscountTags.Any(x => x.Id == discountTag.Id && !x.IsDeleted))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.DiscountTags discountTagExisting = context.DiscountTags.First(x => x.Id == discountTag.Id);
                discountTagExisting.IsActive = discountTag.IsActive;
                discountTagExisting.ModifiedOn = DateTime.Now;
                discountTagExisting.ModifiedBy = discountTag.ModifiedBy;
                //discountTagExisting.CreatedUser = null;
                //discountTagExisting.ModifiedUser = null;

                var discountTagUpdated = context.DiscountTags.Attach(discountTagExisting);
                discountTagUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated discountTag as '{discountTag.IsActive}' for id={discountTag.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update discountTag as '{discountTag.IsActive}' for id={discountTag.Id} " + Environment.NewLine +
                    "Invalid/Deleted_DiscountTagId_or_No_data_exists_that_matches_DiscountTagId";
            }
            return result;
        }

        public Result DeleteDiscountTag(DiscountTags discountTag)
        {
            Result result = new Result();
            if (discountTag.Id > 0 && context.DiscountTags.Count() > 0 && context.DiscountTags.Any(x => x.Id == discountTag.Id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.DiscountTags discountTagExisting = context.DiscountTags.First(x => x.Id == discountTag.Id);
                discountTagExisting.IsDeleted = true;
                discountTagExisting.ModifiedOn = DateTime.Now;
                discountTagExisting.ModifiedBy = discountTag.ModifiedBy;
                //discountTagExisting.CreatedUser = null;
                //discountTagExisting.ModifiedUser = null;

                var discountTagUpdated = context.DiscountTags.Attach(discountTagExisting);
                discountTagUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully updated discountTag as deleted for id={discountTag.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update discountTag as deleted for id={discountTag.Id} " + Environment.NewLine +
                    "Invalid_DiscountTagId_or_No_data_exists_that_matches_DiscountTagId";
            }
            return result;
        }

        public Result PermanentDeleteDiscountTag(long id)
        {
            Result result = new Result();
            if (id > 0 && context.DiscountTags.Count() > 0 && context.DiscountTags.Any(x => x.Id == id))
            {
                AirlinesDTOs.DiscountTags discountTag = new AirlinesDTOs.DiscountTags() { Id = id };
                context.DiscountTags.Remove(discountTag);
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully deleted(permanent) discountTag data for id={id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to delete(permanent) discountTag data for id={id} " + Environment.NewLine +
                    "Invalid_DiscountTagId_or_No_data_exists_that_matches_DiscountTagId";
            }
            return result;
        }

        //Airline-DiscountTag Mappings --------------------------------------------------------------------------------
        public bool AddAirlineDiscountTagMappings(List<AirlineDiscountTagMappings> airlineDiscountTagMappings)
        {
            var mapppings = airlineDiscountTagMappings.Where(x => 
                !context.AirlineDiscountTagMappings.Any(y => y.AirlineId == x.AirlineId && y.DiscountTagId == x.DiscountTagId));
            if (mapppings != null && mapppings.Count() > 0)
            {
                context.AirlineDiscountTagMappings.AddRange(mapppings);
                context.SaveChanges();
            }
            return true;
        }

        public bool RemoveAirlineDiscountTagMappings(List<AirlineDiscountTagMappings> airlineDiscountTagMappings)
        {
            var groupedMapping = airlineDiscountTagMappings.Select(x => x.AirlineId + "_" + x.DiscountTagId).Distinct();
            //foreach (var mapping in airlineDiscountTagMappings)
            //{
            //    context.Entry(mapping).State = EntityState.Deleted;
            //    context.SaveChanges();
            //}
            //context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //context.AirlineDiscountTagMappings.RemoveRange(
            //    context.AirlineDiscountTagMappings.Where(x => groupedMapping.Contains(x.AirlineId + "_" + x.DiscountTagId)));
            context.AirlineDiscountTagMappings.RemoveRange(
                context.AirlineDiscountTagMappings.AsEnumerable().Where(x => 
                airlineDiscountTagMappings.Any(y => y.AirlineId == x.AirlineId && y.DiscountTagId == x.DiscountTagId)));
            context.SaveChanges();
            return true;
        }

        public IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappings(long? airlineId = null, long? discountId = null)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if ((airlineId == null || airlineId <= 0) && (discountId == null || discountId <= 0))
                return context.AirlineDiscountTagMappings.Include(x => x.Airline).Include(x => x.DiscountTag).AsEnumerable();
            else
            {
                var res = context.AirlineDiscountTagMappings.Include(x => x.Airline).Include(x => x.DiscountTag).Where(x =>
                     (airlineId == null || airlineId <= 0 || x.AirlineId == airlineId)
                     && (discountId == null || discountId <= 0 || x.DiscountTagId == discountId)).ToList();
                return res;
            }
        }

        public IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappingsByIds(List<long> ids, bool isByAirlineId)
        {
            throw new NotImplementedException();
        }

        //Airline-DiscountTag Mappings --------------------------------------------------------------------------------
        public IEnumerable<AirlineSchedules> GetAirlineSchedules(long? id = null, bool isByAirlineId = false)
        {
            if (id == null)
                return context.AirlineSchedules.Include(x => x.Airline).Where(x => !x.IsDeleted);
            else
                return context.AirlineSchedules.Include(x => x.Airline).Where(x => !x.IsDeleted && (isByAirlineId ? x.AirlineId == id : x.Id == id));
        }

        public IEnumerable<AirlineSchedules> GetAirlineSchedulesByIds(List<long> ids, bool isByAirlineId = false)
        {
            if (ids == null || ids.Count() <= 0)
                return context.AirlineSchedules.Include(x => x.Airline).Where(x => !x.IsDeleted);
            else
                return context.AirlineSchedules.Include(x => x.Airline).AsEnumerable().Where(x => !x.IsDeleted 
                && ids.Contains((isByAirlineId ? x.AirlineId : x.Id)));
        }

        public IEnumerable<AirlineSchedules> GetGetAirlineSchedulesByFilterCondition(AirlineScheduleDetails schedule)
        {
            if (schedule == null || (schedule.Id <= 0 && string.IsNullOrWhiteSpace(schedule.From) && string.IsNullOrWhiteSpace(schedule.To)
                    && (schedule.DepartureDay == null || !Enum.IsDefined(typeof(DayOfWeek), schedule.DepartureDay))
                    && schedule.DepartureDate == null && schedule.DepartureTime == null
                    && (schedule.ArrivalDay == null || !Enum.IsDefined(typeof(DayOfWeek), schedule.ArrivalDay))
                    && schedule.DepartureDate == null && schedule.DepartureTime == null))
                return context.AirlineSchedules.Where(x => !x.IsDeleted);
            return context.AirlineSchedules.Include(x => x.Airline).AsNoTracking().Where(x => !x.IsDeleted
                && (schedule.Id <= 0 || schedule.Id == x.Id)
                && (string.IsNullOrWhiteSpace(schedule.From) || x.From.Contains(schedule.From))
                && (string.IsNullOrWhiteSpace(schedule.To) || x.To.Contains(schedule.To))
                && ((x.IsRegular && (schedule.DepartureDay == null || !Enum.IsDefined(typeof(DayOfWeek), schedule.DepartureDay)
                    //|| x.DepartureDay.Equals(schedule.DepartureDay)))
                    || (int)x.DepartureDay >= (int)schedule.DepartureDay))
                    || (!x.IsRegular && (schedule.DepartureDate == null
                    //|| x.DepartureDate.Value.Date.Equals(schedule.DepartureDate.Value.Date)))
                    || x.DepartureDate.Value.Date >= schedule.DepartureDate.Value.Date))
                    )
                //&& (!x.IsRegular && (schedule.DepartureDate == null
                //    //|| x.DepartureDate.Value.Date.Equals(schedule.DepartureDate.Value.Date)))
                //    || x.DepartureDate.Value.Date >= schedule.DepartureDate.Value.Date))
                && (schedule.DepartureTime == null || (x.DepartureTime.Hour >= schedule.DepartureTime.Value.Hour
                    && x.DepartureTime.Minute >= schedule.DepartureTime.Value.Minute))
                //&& (schedule.DepartureTime == null || x.DepartureTime.ToShortTimeString().Equals(schedule.DepartureTime.Value.ToShortTimeString()))
                && ((x.IsRegular && (schedule.ArrivalDay == null || !Enum.IsDefined(typeof(DayOfWeek), schedule.ArrivalDay)
                    //|| x.ArrivalDay.Equals(schedule.ArrivalDay)))
                    || (int)x.ArrivalDay <= (int)schedule.ArrivalDay))
                    || (!x.IsRegular && (schedule.ArrivalDate == null
                        //|| x.ArrivalDate.Value.Date.Equals(schedule.ArrivalDate.Value.Date)))
                        || x.ArrivalDate.Value.Date <= schedule.ArrivalDate.Value.Date))
                    )
                //&& (!x.IsRegular && (schedule.ArrivalDate == null
                //    //|| x.ArrivalDate.Value.Date.Equals(schedule.ArrivalDate.Value.Date)))
                //    || x.ArrivalDate.Value.Date <= schedule.ArrivalDate.Value.Date))
                && (schedule.ArrivalTime == null || (x.ArrivalTime.Hour <= schedule.ArrivalTime.Value.Hour
                    && x.ArrivalTime.Minute <= schedule.ArrivalTime.Value.Minute))
                //&& (schedule.ArrivalTime == null || x.ArrivalTime.ToShortTimeString().Equals(schedule.ArrivalTime.Value.ToShortTimeString()))
                ).ToList();
        }

        public IEnumerable<AirlineSchedules> GetGetAirlineSchedulesByMultipleFilterConditions(List<AirlineScheduleDetails> schedules)
        {
            throw new NotImplementedException();
        }

        public long AddAirlineSchedule(AirlineSchedules schedule)
        {
            schedule.CreatedOn = DateTime.Now;
            schedule.ModifiedOn = DateTime.Now;
            schedule.Airline = null;
            //airline.CreatedUser = null;
            //airline.ModifiedUser = null;
            context.AirlineSchedules.Add(schedule);
            context.SaveChanges();
            long id = schedule.Id;
            return id;
        }

        public List<long> AddAirlineSchedulesByRange(List<AirlineSchedules> schedules)
        {
            context.AirlineSchedules.AddRange(schedules);
            context.SaveChanges();
            List<long> ids = schedules.Select(x => x.Id).ToList();
            return ids;
        }

        public bool IsAirlineScheduleAlreadyExists(AirlineSchedules schedule)
        {
            return context.AirlineSchedules.Any(x => //(schedule.Id <= 0 || x.Id != schedule.Id) &&
                  x.AirlineId == schedule.AirlineId 
                 && !x.IsDeleted
                 && x.From.Equals(schedule.From) && x.To.Equals(schedule.To)
                 && ((x.DepartureDay != null && x.DepartureDay.Equals(schedule.DepartureDay))
                     || (x.DepartureDate != null && x.DepartureDate.Value.Date.Equals(schedule.DepartureDate.Value.Date)))
                 && ((x.ArrivalDay != null && x.ArrivalDay.Equals(schedule.ArrivalDay))
                     || (x.ArrivalDate != null && x.ArrivalDate.Value.Date.Equals(schedule.ArrivalDate.Value.Date)))
                 && ((x.DepartureTime != null && x.DepartureTime.Hour == schedule.DepartureTime.Hour)
                     || ((x.ArrivalTime != null && x.ArrivalTime.Hour == schedule.ArrivalTime.Hour))));
        }

        public bool IsAirlineScheduleRangeAlreadyExists(List<AirlineSchedules> schedules)
        {
            return context.AirlineSchedules.AsEnumerable().Any(x => !x.IsDeleted
                && schedules.Any(y => //(y.Id <= 0 || x.Id != y.Id) &&
                    x.AirlineId == y.AirlineId 
                    && x.From.Equals(y.From) && x.To.Equals(y.To)
                    && ((x.DepartureDay != null && x.DepartureDay.Equals(y.DepartureDay))
                        || (x.DepartureDate != null && x.DepartureDate.Value.Date.Equals(y.DepartureDate.Value.Date)))
                    && ((x.ArrivalDay != null && x.ArrivalDay.Equals(y.ArrivalDay))
                        || (x.ArrivalDate != null && x.ArrivalDate.Value.Date.Equals(y.ArrivalDate.Value.Date)))
                    && ((x.DepartureTime != null && x.DepartureTime.Hour == y.DepartureTime.Hour)
                        || ((x.ArrivalTime != null && x.ArrivalTime.Hour == y.ArrivalTime.Hour)))));
        }

        public bool DeleteAirlineSchedule(long id, long userId)
        {
            bool result = false;
            if (id > 0 && context.AirlineSchedules.Count() > 0 && context.AirlineSchedules.Any(x => x.Id == id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlinesDTOs.AirlineSchedules scheduleExisting = context.AirlineSchedules.First(x => x.Id == id);
                scheduleExisting.IsDeleted = true;
                scheduleExisting.ModifiedOn = DateTime.Now;
                scheduleExisting.ModifiedBy = userId;
                //scheduleExisting.CreatedUser = null;
                //scheduleExisting.ModifiedUser = null;

                var scheduleUpdated = context.AirlineSchedules.Attach(scheduleExisting);
                scheduleUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool DeleteAirlineScheduleByScheduleIds(List<long> ids, long userId)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            bool result = false;
            if (ids != null && ids.Count() > 0 && context.AirlineSchedules.Count() > 0)
            {
                //var schedules = context.AirlineSchedules.AsNoTracking().Where(x => ids.Contains(x.Id));
                //if (schedules != null && schedules.Count() > 0)
                //{
                foreach (var id in ids)
                {
                    var schedule = context.AirlineSchedules.AsNoTracking().FirstOrDefault(x => x.Id == id);
                    if (schedule != null && schedule.Id > 0)
                    {
                        schedule.IsDeleted = true;
                        schedule.ModifiedOn = DateTime.Now;
                        schedule.ModifiedBy = userId;
                        var scheduleUpdated = context.AirlineSchedules.Attach(schedule);
                        scheduleUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        context.SaveChanges();
                        result = true;
                    }
                }
                //}
            }
            return result;
        }

        public Result PermanentDeleteAirlineSchedule(long id)
        {
            Result result = new Result();
            if (id > 0 && context.AirlineSchedules.Count() > 0 && context.AirlineSchedules.Any(x => x.Id == id))
            {
                AirlinesDTOs.AirlineSchedules schedule = new AirlinesDTOs.AirlineSchedules() { Id = id };
                context.AirlineSchedules.Remove(schedule);
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully deleted(permanent) AirlineSchedule for id={id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to delete(permanent) AirlineSchedule data for id={id} " + Environment.NewLine +
                    "Invalid_ScheduleId_or_No_data_exists_that_matches_ScheduleId";
            }
            return result;
        }

        //AirlineSchedule-Tracker --------------------------------------------------------------------------------

        public IEnumerable<AirlineScheduleTracker> GetAirlineScheduleTracker(long? id = null, bool isByScheduleId = false)
        {
            if (id == null)
                return context.AirlineScheduleTracker.Include(x => x.AirlineSchedule).Include(x => x.AirlineSchedule.Airline)
                    .Where(x => !x.IsDeleted);
            else
                return context.AirlineScheduleTracker.Include(x => x.AirlineSchedule).Include(x => x.AirlineSchedule.Airline)
                    .Where(x => !x.IsDeleted && (isByScheduleId ? x.ScheduleId == id : x.Id == id));
        }

        public IEnumerable<AirlineScheduleTracker> GetAirlineScheduleTrackerByIds(List<long> ids, bool isByScheduleId = false)
        {
            if (ids == null || ids.Count() <= 0)
                return context.AirlineScheduleTracker.Include(x => x.AirlineSchedule).Include(x => x.AirlineSchedule.Airline)
                    .Where(x => !x.IsDeleted);
            else
                return context.AirlineScheduleTracker.Include(x => x.AirlineSchedule).Include(x => x.AirlineSchedule.Airline)
                    .AsEnumerable().Where(x => !x.IsDeleted && ids.Contains((isByScheduleId ? x.ScheduleId : x.Id)));
        }

        public IEnumerable<AirlineScheduleTracker> GetAirlineScheduleTrackerByFilterCondition(AirlineScheduleTracker scheduleTracker)
        {
            if (scheduleTracker == null || (scheduleTracker.Id <= 0 && scheduleTracker.ScheduleId <= 0 
                && scheduleTracker.ActualDepartureDate == null && scheduleTracker.ActualArrivalDate == null))
                return context.AirlineScheduleTracker.Where(x => !x.IsDeleted);
            return context.AirlineScheduleTracker.Include(x => x.AirlineSchedule).Include(x => x.AirlineSchedule.Airline).AsEnumerable()
                .Where(x => !x.IsDeleted
                && (scheduleTracker.Id <= 0 || scheduleTracker.Id == x.Id)
                && (scheduleTracker.ScheduleId <= 0 || scheduleTracker.ScheduleId == x.ScheduleId)
                && (scheduleTracker.ActualDepartureDate == null 
                    || (scheduleTracker.ActualDepartureDate.Date.Equals(x.ActualDepartureDate.Date)
                        && scheduleTracker.ActualDepartureDate.ToShortTimeString().Equals(x.ActualDepartureDate.ToShortTimeString())))
                && (scheduleTracker.ActualArrivalDate == null 
                    || (scheduleTracker.ActualArrivalDate.Value.Date.Equals(x.ActualArrivalDate.Value.Date)
                        && scheduleTracker.ActualArrivalDate.Value.ToShortTimeString().Equals(x.ActualArrivalDate.Value.ToShortTimeString())))
                );
        }

        public long AddAirlineScheduleTracker(AirlineScheduleTracker scheduleTracker)
        {
            scheduleTracker.AirlineSchedule = null;
            context.AirlineScheduleTracker.Add(scheduleTracker);
            context.SaveChanges();
            long id = scheduleTracker.Id;
            return id;
        }

        public bool UpdateAirlineScheduleTracker(long id, int bcTickets, int nbcTickets, bool isRevert = false)
        {
            bool result = false;
            if (id > 0 && (bcTickets > 0 || nbcTickets > 0) && context.AirlineScheduleTracker.Count() > 0
                && context.AirlineScheduleTracker.Any(x => x.Id == id && !x.IsDeleted))
            {
                using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    AirlineScheduleTracker scheduleTrackerExisting = context.AirlineScheduleTracker.First(x => x.Id == id);
                    if (bcTickets > 0)
                    {
                        if(isRevert)
                            scheduleTrackerExisting.BCSeatsRemaining = scheduleTrackerExisting.BCSeatsRemaining + bcTickets;
                        else
                            scheduleTrackerExisting.BCSeatsRemaining = scheduleTrackerExisting.BCSeatsRemaining - bcTickets;
                    }
                    if (nbcTickets > 0)
                    {
                        if(isRevert)
                            scheduleTrackerExisting.NBCSeatsRemaining = scheduleTrackerExisting.NBCSeatsRemaining + nbcTickets;
                        else
                            scheduleTrackerExisting.NBCSeatsRemaining = scheduleTrackerExisting.NBCSeatsRemaining - nbcTickets;
                    }

                    var scheduleTrackerUpdated = context.AirlineScheduleTracker.Attach(scheduleTrackerExisting);
                    scheduleTrackerUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();

                    transaction.Commit();
                    result = true;
                }
            }
            return result;
        }

        public bool DeleteAirlineScheduleTracker(long id)
        {
            bool result = false;
            if (id > 0 && context.AirlineScheduleTracker.Count() > 0 && context.AirlineScheduleTracker.Any(x => x.Id == id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                AirlineScheduleTracker scheduleTrackerExisting = context.AirlineScheduleTracker.First(x => x.Id == id);
                scheduleTrackerExisting.IsDeleted = true;

                var scheduleTrackerUpdated = context.AirlineScheduleTracker.Attach(scheduleTrackerExisting);
                scheduleTrackerUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool DeleteAirlineScheduleTrackerByTrackerIds(List<long> ids)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            bool result = false;
            if (ids != null && ids.Count() > 0 && context.AirlineScheduleTracker.Count() > 0)
            {
                foreach (var id in ids)
                {
                    var scheduleTracker = context.AirlineScheduleTracker.AsNoTracking().FirstOrDefault(x => x.Id == id);
                    if (scheduleTracker != null && scheduleTracker.Id > 0)
                    {
                        scheduleTracker.IsDeleted = true;
                        var scheduleTrackerUpdated = context.AirlineScheduleTracker.Attach(scheduleTracker);
                        scheduleTrackerUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        context.SaveChanges();
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
