using CommonDTOs;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsersDTOs;
using UsersRepositoryUtils.DBContext;

namespace UsersRepositoryUtils.DBHelper
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDBContext context;

        public UsersRepository(UsersDBContext context)
        {
            this.context = context;
        }
        public long Register(UsersDTOs.Users user)
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

        public UsersDTOs.Users ValidateLoginAndUdpateAccountStatus(UsersDTOs.Users user)
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

        public UsersDTOs.Users ValidateLogin(UsersDTOs.Users user)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var userExisting = context.Users.FirstOrDefault(x =>
                x.EmailId.ToUpper().Equals(user.EmailId.ToUpper())
                && x.Password.ToUpper().Equals(user.Password.ToUpper())
                && (x.AccountStatusId == (int)AccountStatusCode.Active || x.AccountStatusId == (int)AccountStatusCode.Registered)
                && !x.IsDeleted);
            if(userExisting != null)
                context.Entry(userExisting).State = EntityState.Detached;
            return userExisting;
        }

        public bool UserExists(string emailId, long? id = null)
        {
            return context.Users.Any(u => u.EmailId.Equals(emailId) && (id == null || id != u.Id) && !u.IsDeleted);
        }
        public bool ValidateAdmin(UsersDTOs.Users user)
        {
            return context.Users.Any(u => (user.Id <= 0 || u.Id == user.Id)
                && (string.IsNullOrWhiteSpace(user.EmailId) || u.EmailId.Equals(user.EmailId))
                && (u.AccountStatusId == (int)AccountStatusCode.Active)
                && (u.IsSuperAdmin == true)
                && !u.IsDeleted);
        }

        public IEnumerable<UsersDTOs.Users> GetUsers(long? id = null)
        {
            try
            {
                var users = new List<UsersDTOs.Users>();
                if (id == null || id <= 0)
                    users = context.Users.Include(x => x.AccountStatus).Where(x => !x.IsDeleted).ToList();
                else
                {
                    users = new List<UsersDTOs.Users>() { context.Users.Include(x => x.AccountStatus)
                        .FirstOrDefault(x => x.Id == id && !x.IsDeleted) }.ToList();
                }
                //context.Entry(users).State = EntityState.Detached;
                return users.ToList();
            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }

        public UsersDTOs.Users GetUserByEmailId(string emailId)
        {
            var users = context.Users.Include(x => x.AccountStatus)
                    .FirstOrDefault(x => x.EmailId.Equals(emailId ?? string.Empty) && !x.IsDeleted);
            context.Entry(users).State = EntityState.Detached;
            return users;
        }

        public IEnumerable<UsersDTOs.Users> GetUsersByFilterCondition(UsersDTOs.UserDetails user = null)
        {
            try
            {
                var users = new List<UsersDTOs.Users>();
                if (user == null || (user.Id <= 0 && string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName)
                    && string.IsNullOrWhiteSpace(user.EmailId) && user.AccountStatusId <= 0))
                    users = context.Users.Include(x => x.AccountStatus).Where(x => !x.IsDeleted).ToList();
                else
                    users= context.Users.Include(x => x.AccountStatus).Where(x => (user.Id <= 0 || user.Id == x.Id)
                        && (string.IsNullOrWhiteSpace(user.FirstName) || x.FirstName.Contains(user.FirstName))
                        && (string.IsNullOrWhiteSpace(user.LastName) || x.LastName.Contains(user.LastName))
                        && (string.IsNullOrWhiteSpace(user.EmailId) || x.LastName.Contains(user.EmailId))
                        && x.IsSuperAdmin == user.IsSuperAdmin
                        && (user.AccountStatusId <= 0 || user.AccountStatusId == x.AccountStatusId)
                        && !x.IsDeleted
                        ).ToList();
                //context.Entry(users).State = EntityState.Detached;
                return users;
            }
            catch (Exception ex)
            {
                //printlog
                throw;
            }
        }

        public Result UpdateUser(UsersDTOs.Users user)
        {
            try
            {
                Result result = new Result();
                if (user.Id > 0 && context.Users.Count() > 0 && context.Users.Any(x => x.Id == user.Id && !x.IsDeleted))
                {

                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    UsersDTOs.Users userExisting = context.Users.First(x => x.Id == user.Id && !x.IsDeleted);
                    //context.Entry(userExisting).State = EntityState.Detached;
                    if (!string.IsNullOrWhiteSpace(user.FirstName))
                        userExisting.FirstName = user.FirstName;
                    if (!string.IsNullOrWhiteSpace(user.LastName))
                        userExisting.LastName = user.LastName;
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
                        "Invalid/Deleted_UserId_or_No_data_exists_that_matches_UserId";
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
                if (id > 0 && context.Users.Count() > 0 && context.Users.Any(x => x.Id == id && !x.IsDeleted))
                {
                    context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    UsersDTOs.Users userExisting = context.Users.First(x => x.Id == id && !x.IsDeleted);
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
                        "Invalid/Deleted_UserId_or_No_data_exists_that_matches_UserId";
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
                    UsersDTOs.Users userExisting = context.Users.First(x => x.Id == id);
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
                    UsersDTOs.Users user = new UsersDTOs.Users() { Id = id };
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
