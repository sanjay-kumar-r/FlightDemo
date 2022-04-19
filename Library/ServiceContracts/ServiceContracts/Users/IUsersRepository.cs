using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;
using UserDtOs;

namespace ServiceContracts
{
    public interface IUsersRepository
    {
        long Register(Users user);

        Users ValidateLoginAndUdpateAccountStatus(Users user);

        bool UserExists(string emailId, long? id = null);

        bool ValidateAdmin(UserDtOs.Users user);

        IEnumerable<UserDtOs.Users> GetUsers(long? id = null);

        IEnumerable<UserDtOs.Users> GetUsersByFiltercondition(UserDtOs.Users user = null);

        Result UpdateUser(UserDtOs.Users user);

        Result UpdateUserAsSuperAdmin(long id);

        Result DeteteUser(long id);

        Result PermanentDeleteUser(long id);
    }
}
