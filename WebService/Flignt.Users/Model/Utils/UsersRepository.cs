using CommonDTOs;
using ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UserDtOs;

namespace Flight.Users.Model.Utils
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDBContext context;

        public UsersRepository(UsersDBContext context)
        {
            this.context = context;
        }
        public long Register(UserDtOs.Users user)
        {
            user.CreatedOn = DateTime.Now;
            user.ModifiedOn = DateTime.Now;
            user.AccountStatusId = context.AccountStatus.First(x => x.Id == (int)AccountStatusCode.Registered).Id;
            user.AccountStatus = null;
            context.Users.Add(user);
            context.SaveChanges();
            long temp = user.Id;
            long id = context.Users.OrderBy(x => x.Id).LastOrDefault().Id;
            return id;
        }

        public UserDtOs.Users ValidateLoginAndUdpateAccountStatus(UserDtOs.Users user)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var userExisting = context.Users.FirstOrDefault(x =>
                x.EmailId.ToUpper().Equals(user.EmailId.ToUpper())
                && x.Password.ToUpper().Equals(user.Password.ToUpper())
                && (x.AccountStatusId == (int)AccountStatusCode.Active || x.AccountStatusId == (int)AccountStatusCode.Registered)
                && !x.IsDeleted);
            if (userExisting != null && userExisting.Id > 0) 
            {
                if (userExisting.AccountStatusId == (int)AccountStatusCode.Registered)
                {
                    userExisting.AccountStatusId = (int)AccountStatusCode.Active;
                    userExisting.ModifiedOn = DateTime.Now;
                    userExisting.AccountStatus = null;
                    var userUpdated = context.Users.Attach(userExisting);
                    userUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                return userExisting;
            }
            else
                return null;
        }

        public bool UserExists(string emailId, long? id = null)
        {
            return context.Users.Any(u => u.EmailId.Equals(emailId) && (id == null || id != u.Id));
        }
        public bool ValidateAdmin(UserDtOs.Users user)
        {
            return context.Users.Any(u => (user.Id <= 0 || u.Id == user.Id) 
                &&(string.IsNullOrWhiteSpace(user.EmailId) ||  u.EmailId.Equals(user.EmailId))
                && (u.AccountStatusId == (int)AccountStatusCode.Active)
                && (u.IsSuperAdmin == true));
        }

        public IEnumerable<UserDtOs.Users> GetUsers(long? id = null)
        {
            try
            {
                if (id == null)
                    return context.Users.Include(x => x.AccountStatus).ToList();
                else
                    return new List<UserDtOs.Users>() { context.Users.Include(x => x.AccountStatus).FirstOrDefault(x => x.Id == id) };
            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }

        }

        public IEnumerable<UserDtOs.Users> GetUsersByFiltercondition(UserDtOs.Users user = null)
        {
            try
            {
                if (user == null || (user.Id <= 0 && string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName)
                    && string.IsNullOrWhiteSpace(user.EmailId) && user.AccountStatusId <= 0))
                    return context.Users.Include(x => x.AccountStatus).ToList();
                return context.Users.Include(x => x.AccountStatus).Where(x => (user.Id <= 0 || user.Id == x.Id)
                    && (string.IsNullOrWhiteSpace(user.FirstName) || x.FirstName.Contains(user.FirstName))
                    && (string.IsNullOrWhiteSpace(user.LastName) || x.LastName.Contains(user.LastName))
                    && (string.IsNullOrWhiteSpace(user.EmailId) || x.LastName.Contains(user.EmailId))
                    && x.IsSuperAdmin == user.IsSuperAdmin
                    && (user.AccountStatusId <= 0 || user.AccountStatusId == x.AccountStatusId)
                    //&& !x.IsDeleted
                    );
            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }

        public Result UpdateUser(UserDtOs.Users user)
        {
            try
            {
                Result result = new Result();
                if (user.Id > 0 && context.Users.Count() > 0 && context.Users.Any(x => x.Id == user.Id))
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    UserDtOs.Users userExisting = context.Users.First(x => x.Id == user.Id);
                    if(!string.IsNullOrWhiteSpace(user.FirstName))
                        userExisting.FirstName = user.FirstName;
                    if (!string.IsNullOrWhiteSpace(user.LastName))
                        userExisting.LastName = user.LastName ;
                    if (!string.IsNullOrWhiteSpace(user.EmailId))
                        userExisting.EmailId = user.EmailId;
                    if (!string.IsNullOrWhiteSpace(user.Password))
                        userExisting.Password = user.Password;
                    if (Enum.IsDefined(typeof(AccountStatusCode), user.AccountStatusId))
                        userExisting.AccountStatusId = user.AccountStatusId;
                    userExisting.ModifiedOn = DateTime.Now;
                    userExisting.AccountStatus = null;

                    var userUpdated = context.Users.Attach(userExisting);
                    userUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                    result.Res = true;
                    result.ResultMessage = $"successfully updated user date for id={user.Id}";
                }
                else
                {
                    result.Res = false;
                    result.ResultMessage = $"unable to update user date for id={user.Id} " + Environment.NewLine +
                        "Invalid_UserId_or_No_data_exists_that_matches_UserId";
                }
                return result;

            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }
        public Result UpdateUserAsSuperAdmin(long id)
        {
            try
            {
                Result result = new Result();
                if (id > 0 && context.Users.Count() > 0 && context.Users.Any(x => x.Id == id))
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    UserDtOs.Users userExisting = context.Users.First(x => x.Id == id);
                    userExisting.IsSuperAdmin = true;
                    userExisting.ModifiedOn = DateTime.Now;
                    userExisting.AccountStatus = null;
                    var userUpdated = context.Users.Attach(userExisting);
                    userUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                    result.Res = true;
                    result.ResultMessage = $"successfully updated user as super admin for id={id}";
                }
                else
                {
                    result.Res = false;
                    result.ResultMessage = $"unable to update user as super admin for id={id} " + Environment.NewLine +
                        "Invalid_UserId_or_No_data_exists_that_matches_UserId";
                }
                return result;

            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }

        public Result DeteteUser(long id)
        {
            try
            {
                Result result = new Result();
                if (id > 0 && context.Users.Count() > 0 && context.Users.Any(x => x.Id == id))
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    UserDtOs.Users userExisting = context.Users.First(x => x.Id == id);
                    userExisting.IsDeleted = true;
                    userExisting.ModifiedOn = DateTime.Now;
                    userExisting.AccountStatus = null;
                    var userUpdated = context.Users.Attach(userExisting);
                    userUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                    result.Res = true;
                    result.ResultMessage = $"successfully updated user as deleted for id={id}";
                }
                else
                {
                    result.Res = false;
                    result.ResultMessage = $"unable to update user as deleted for id={id} \n " +
                        "Invalid_UserId_or_No_data_exists_that_matches_UserId";
                }
                return result;

            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }

        //Permanantdelete
        public Result PermanentDeleteUser(long id)
        {
            try
            {
                Result result = new Result();
                if (id > 0 && context.Users.Count() > 0 && context.Users.Any(x => x.Id == id))
                {
                    UserDtOs.Users user = new UserDtOs.Users() { Id = id };
                    context.Users.Remove(user);
                    context.SaveChanges();
                    result.Res = true;
                    result.ResultMessage = $"successfully deleted(permanent) user date for id={id}";
                }
                else
                {
                    result.Res = false;
                    result.ResultMessage = $"unable to delete(permanent) user date for id={id} " + Environment.NewLine +
                        "Invalid_UserId_or_No_data_exists_that_matches_UserId";
                }
                return result;
            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }
    }
}
