using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;
using UsersDTOs;

namespace ServiceContracts.Users
{
    public interface IUsersRepository
    {
        long Register(UsersDTOs.Users user);

        UsersDTOs.Users ValidateLogin(UsersDTOs.Users user);

        UsersDTOs.Users ValidateLoginAndUdpateAccountStatus(UsersDTOs.Users user);

        bool UserExists(string emailId, long? id = null);

        bool ValidateAdmin(UsersDTOs.Users user);

        IEnumerable<UsersDTOs.Users> GetUsers(long? id = null);

        IEnumerable<UsersDTOs.Users> GetUsersByFilterCondition(UsersDTOs.UserDetails user = null);

        UsersDTOs.Users GetUserByEmailId(string emailId);

        Result UpdateUser(UsersDTOs.Users user);

        Result UpdateUserAsSuperAdmin(long id);

        Result DeteteUser(long id);

        Result PermanentDeleteUser(long id);
    }
}
