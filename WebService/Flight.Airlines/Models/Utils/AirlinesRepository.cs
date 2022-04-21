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
        }

        //Airlines
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
                && (airline.Id <= 0 || airline.Id != airline.Id) && !x.IsDeleted);
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
                return new List<AirlinesDTOs.Airlines>() { context.Airlines.FirstOrDefault(x => x.Id == id) };
        }

        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition(AirlinesDTOs.AirlineDetails airline)
        {
            if (airline == null || (airline.Id <= 0 && string.IsNullOrWhiteSpace(airline.Name) && string.IsNullOrWhiteSpace(airline.AirlineCode)
                    && airline.BCTicketCost <= 0 && airline.NBCTicketCost <= 0) || airline.IsActive == null)
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

        public IEnumerable<DiscountTags> GetAirlinesByMultipleFilterconditions(List<AirlineDetails> airlineDetails)
        {
            if (airlineDetails == null || airlineDetails.Count() <= 0)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            Guid g = Guid.NewGuid();
            string splitter = g.ToString();
            g = Guid.NewGuid();
            string idPlaceHolder = g.ToString();
            g = Guid.NewGuid();
            string namePlaceHolder = g.ToString();
            g = Guid.NewGuid();
            string codePlaceHolder = g.ToString();
            var discountTagsGrouped = airlineDetails.Select(x => (x.Id > 0 ? x.Id.ToString() : idPlaceHolder) + splitter +
                (!string.IsNullOrWhiteSpace(x.Name) ? x.Name : namePlaceHolder) + splitter +
                (!string.IsNullOrWhiteSpace(x.AirlineCode) ? x.AirlineCode : codePlaceHolder));
            return context.DiscountTags.AsEnumerable().Where(x => !x.IsDeleted &&
                discountTagsGrouped.Any(y =>
                y.Replace(idPlaceHolder, x.Id.ToString()).Replace(namePlaceHolder, x.Name).Replace(codePlaceHolder, x.DiscountCode)
                .Equals(x.Id + splitter + x.Name + splitter + x.DiscountCode)));
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
                    airlineExisting.BCTicketCost = airline.NBCTicketCost;
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
                result.ResultMessage = $"successfully updated airline date for id={airline.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update airline date for id={airline.Id} " + Environment.NewLine +
                    "Invalid/Deleted_AirlineId_or_No_data_exists_that_matches_AirlineId";
            }
            return result;
        }

        //DiscountTags
        public IEnumerable<DiscountTags> GetDiscountTags(long? id = null)
        {
            if (id == null)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            else
                return new List<AirlinesDTOs.DiscountTags>() { context.DiscountTags.FirstOrDefault(x => x.Id == id) };
        }

        public IEnumerable<DiscountTags> GetDiscountTagsByFiltercondition(DiscountTagDetails discountTag)
        {
            if (discountTag == null || (discountTag.Id <= 0 && string.IsNullOrWhiteSpace(discountTag.Name) && string.IsNullOrWhiteSpace(discountTag.DiscountCode)
                    && discountTag.Discount <= 0) || discountTag.IsActive == null)
                return context.DiscountTags.Where(x => !x.IsDeleted);
            return context.DiscountTags.Where(x => (discountTag.Id <= 0 || discountTag.Id == x.Id)
                && (string.IsNullOrWhiteSpace(discountTag.Name) || x.Name.Contains(discountTag.Name))
                && (string.IsNullOrWhiteSpace(discountTag.DiscountCode) || x.DiscountCode.Contains(discountTag.DiscountCode))
                && (discountTag.Discount <= 0 || x.Discount <= discountTag.Discount)
                && (discountTag.IsActive == null || x.IsActive == discountTag.IsActive)
                && !x.IsDeleted
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
                result.ResultMessage = $"successfully updated discountTag date for id={discountTag.Id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to update discountTag date for id={discountTag.Id} " + Environment.NewLine +
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
                result.ResultMessage = $"successfully deleted(permanent) discountTag date for id={id}";
            }
            else
            {
                result.Res = false;
                result.ResultMessage = $"unable to delete(permanent) discountTag date for id={id} " + Environment.NewLine +
                    "Invalid_DiscountTagId_or_No_data_exists_that_matches_DiscountTagId";
            }
            return result;
        }

        public Result AddAirlineDiscountTagMappings(List<AirlineDiscountTagMappings> airlineDiscountTagMappings)
        {
            Result result = new Result();
            var mapppings = airlineDiscountTagMappings.Where(x => 
                !context.AirlineDiscountTagMappings.Any(y => y.AirlineId == x.AirlineId && y.DiscountTagId == x.DiscountTagId));
            if (mapppings != null && mapppings.Count() > 0)
            {
                context.AirlineDiscountTagMappings.AddRange(mapppings);
                context.SaveChanges();
                result.Res = true;
                result.ResultMessage = $"successfully added airlineDiscountTagMappings";
            }
            else
            {
                result.Res = true;
                result.ResultMessage = $"airlineDiscountTagMappings already exists";
            }
            return result;
        }

        public IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappings(long? airlineId = null, long? discountId = null)
        {
            if ((airlineId == null || airlineId <= 0) && (discountId == null || discountId <= 0)  )
                return context.AirlineDiscountTagMappings.Include(x => x.Airline).Include(x => x.DiscountTag).AsEnumerable();
            else
                return context.AirlineDiscountTagMappings.Include(x => x.Airline).Include(x => x.DiscountTag).Where(x => 
                    (airlineId == null || airlineId <= 0 || x.AirlineId == airlineId) 
                    && (discountId == null || discountId <= 0 || x.DiscountTagId == discountId));
        }

        public IEnumerable<AirlineDiscountTagMappings> GetAirlineDiscountTagsMappingsByAirlineIds(List<long> airlineIds)
        {
            throw new NotImplementedException();
        }
    }
}
