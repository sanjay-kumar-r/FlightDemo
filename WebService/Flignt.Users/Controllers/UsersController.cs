using CommonDTOs;
using Flight.Users.Model.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDtOs;

namespace Flight.Users.Controllers
{
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IUsersRepository usersRepo;

        public UsersController(IConfiguration config, IUsersRepository usersRepo)
        {
            this.config = config;
            this.usersRepo = usersRepo;
        }

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "UsersController -> Pong";
        }

        [HttpGet]
        public IEnumerable<UserDtOs.Users> Get()
        {
            return usersRepo.GetUsers();
        }

        [HttpGet]
        [Route("{id}")]
        [Route("GetUsers/{id}")]
        public IEnumerable<UserDtOs.Users> Get(long id)
        {
            return usersRepo.GetUsers(id);
        }

        [HttpPost]
        [Route("GetUsersByFiltercondition")]
        public IEnumerable<UserDtOs.Users> GetUsersByFiltercondition([FromBody] UserDtOs.Users user)
        {
            return usersRepo.GetUsersByFiltercondition(user);
        }

        [HttpPost]
        [Route("Register")]
        public long Register([FromBody] UserDtOs.Users user)
        {
            if (!UsersValidation.ValidateRegistration(user))
                throw new Exception("UsersValidation.ValidateRegistration Falied");

            if (usersRepo.UserExists(user.EmailId))
                throw new Exception("User with same emailId already exists");
            return usersRepo.Register(user);
        }

        [HttpPost]
        [Route("RegisterAsAdmin")]
        public long RegisterAsAdmin([FromBody] UserDtOs.Users user)
        {
            if (!UsersValidation.ValidateRegistration(user))
                throw new Exception("UsersValidation.ValidateRegistration Falied");

            if (usersRepo.UserExists(user.EmailId))
                throw new Exception("User with same emailId already exists");
            user.IsSuperAdmin = true;
            return usersRepo.Register(user);
        }

        [HttpPost]
        [Route("ValidateAdmin")]
        public bool ValidateAdmin([FromBody] long id)
        {
            UserDtOs.Users user = new UserDtOs.Users() { Id = id };
            return usersRepo.ValidateAdmin(user);
        }

        [HttpPost]
        [Route("Login")]
        public UserDtOs.Users Login([FromBody] UserDtOs.Users user)
        {
            if(!UsersValidation.ValidateLogin(user))
                throw new Exception("UsersValidation.ValidateLogin Falied");

            var userDetails = usersRepo.ValidateLoginAndUdpateAccountStatus(user);
            if (userDetails != null)
                return userDetails;
            else
                throw new Exception("Invalid email and/or password");
        }

        [HttpPost]
        [Route("Update")]
        public Result Update([FromBody] UserDtOs.Users user)
        {
            if (!UsersValidation.ValidateUpdate(user))
                throw new Exception("UsersValidation.ValidateUpdate Falied");

            if (usersRepo.UserExists(user.EmailId, user.Id))
                throw new Exception("User with same emailId already exists");
            return usersRepo.UpdateUser(user);
        }

        [HttpPost]
        [Route("UpdateUserAsSuperAdmin")]
        public Result UpdateUserAsSuperAdmin([FromBody] long id)
        {
            return usersRepo.UpdateUserAsSuperAdmin(id);
        }

        [HttpPost]
        [Route("Delete")]
        public Result Delete([FromBody]long id)
        {
            return usersRepo.DeteteUser(id);
        }

        [HttpPost]
        [Route("PermanentDeleteUser")]
        public Result PermanentDeleteUser([FromBody] long id)
        {
            return usersRepo.PermanentDeleteUser(id);
        }
    }
}
