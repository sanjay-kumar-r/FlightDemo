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
    public class DiscountTagsRepository : IDiscountTagsRepository
    {
        private readonly AirlinesDBContext context;

        public DiscountTagsRepository(AirlinesDBContext context)
        {
            this.context = context;
        }

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

        public bool DiscountTagExists(DiscountTags discountTag)
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

        Result IDiscountTagsRepository.PermanentDelete(long id)
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
    }
}
