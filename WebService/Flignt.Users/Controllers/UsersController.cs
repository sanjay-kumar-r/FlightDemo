using AuthDTOs;
using CommonDTOs;
using CommonUtils.APIExecuter;
using Flight.Users.Model.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceContracts;
using ServiceContracts.Logger;
using ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersDTOs;

namespace Flight.Users.Controllers
{
    [AllowAnonymous]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CustomSettings customSettings;
        //private readonly IConfiguration config;
        private readonly IUsersRepository usersRepo;
        private readonly ILogger logger;

        public UsersController(IConfiguration config, IUsersRepository usersRepo, ILogger logger)
        {
            customSettings = new CustomSettings();
            config.GetSection("CustomSettings").Bind(customSettings);
            //this.config = config;
            this.usersRepo = usersRepo;
            this.logger = logger;
        }

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "UsersController -> Pong";
        }

        [HttpGet]
        public IEnumerable<UsersDTOs.Users> Get()
        {
            return usersRepo.GetUsers();
        }

        [HttpGet]
        [Route("{id}")]
        //[Route("GetUsers/{id}")]
        public IEnumerable<UsersDTOs.Users> Get(long id)
        {
            return usersRepo.GetUsers(id);
        }

        [HttpPost]
        [Route("GetUsersByFilterCondition")]
        public IEnumerable<UsersDTOs.Users> GetUsersByFilterCondition([FromBody] UsersDTOs.Users user)
        {
            var userDetails = new UserDetails()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailId = user.EmailId,
                AccountStatusId = user.AccountStatusId,
                IsSuperAdmin = user.IsSuperAdmin
            };
            return usersRepo.GetUsersByFilterCondition(userDetails);
        }

        [HttpPost]
        [Route("Register")]
        public long Register([FromBody] UsersDTOs.Users user)
        {
            if (!UsersValidation.ValidateRegistration(user))
            {
                logger.Log(LogLevel.ERROR, "UsersValidation.ValidateRegistration Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("UsersValidation.ValidateRegistration Falied");
            }

            if (usersRepo.UserExists(user.EmailId))
            {
                logger.Log(LogLevel.ERROR, "User with same emailId already exists");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Duplicate, CustomErrorMessage = "User with same emailId already exists" };
                //throw new Exception("User with same emailId already exists");
            }

            user.IsSuperAdmin = false;
            user.IsDeleted = false;
            user.AccountStatusId = 0;
            user.AccountStatus = null;
            return usersRepo.Register(user);
        }

        [HttpPost]
        [Route("RegisterAsAdmin")]
        public long RegisterAsAdmin([FromBody] UsersDTOs.Users user)
        {
            if (!UsersValidation.ValidateRegistration(user))
            {
                logger.Log(LogLevel.ERROR, "UsersValidation.ValidateRegistration Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("UsersValidation.ValidateRegistration Falied");
            }

            if (usersRepo.UserExists(user.EmailId))
            {
                logger.Log(LogLevel.ERROR, "User with same emailId already exists");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Duplicate, CustomErrorMessage = "User with same emailId already exists" };
                //throw new Exception("User with same emailId already exists");
            }

            user.IsSuperAdmin = true;
            user.IsDeleted = false;
            user.AccountStatusId = 0;
            user.AccountStatus = null;
            return usersRepo.Register(user);
        }

        [HttpPost]
        [Route("ValidateAdmin")]
        public bool ValidateAdmin([FromBody] long id)
        {
            UsersDTOs.Users user = new UsersDTOs.Users() { Id = id };
            return usersRepo.ValidateAdmin(user);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<UsersDTOs.UserLoginResponse> Login([FromBody] UsersDTOs.Users user)
        {
            if (!UsersValidation.ValidateLogin(user))
            {
                logger.Log(LogLevel.ERROR, "UsersValidation.ValidateLogin Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("UsersValidation.ValidateLogin Falied");
            }

            var userLoginResponse = new UserLoginResponse();
            var userDetail = usersRepo.ValidateLoginAndUdpateAccountStatus(user);
            if (userDetail != null)
            {
                userDetail.Password = null;
                userLoginResponse.User = userDetail;

                string apiGatewayBaseUrl = customSettings.ApiGatewayBaseUrl;
                string getTokenUrl = customSettings.EndpointUrls["GetTokenUrl"];
                string getTokenRequestUrl = apiGatewayBaseUrl.Trim('/', ' ') + "/" + getTokenUrl.Trim('/', ' ');
                var authRequest = new AuthRequest()
                {
                    UserName = user.EmailId,
                    Password = user.Password
                };
                HeaderInfo headerInfo = new HeaderInfo()
                {
                    UserId = HttpContext.Request.Headers["UserId"]
                    //,
                    //TenantId = HttpContext.Request.Headers["TenantId"],
                    //Authorization = HttpContext.Request.Headers["Authorization"],
                    //RefreshToken = HttpContext.Request.Headers["RefreshToken"]
                };
                var authResponse = new AuthResponse();
                using (var response = await (new ApiExecutor(logger)).CallAPI(APIRequestType.Post, getTokenRequestUrl, headerInfo,
                    authRequest, false))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    authResponse = JsonConvert.DeserializeObject<AuthResponse>(apiResponse);
                }
                userLoginResponse.AuthResponse = authResponse;
            }
            else
            {
                //throw new Exception("Invalid email and/or password");
                throw new CustomException()
                {
                    CustomErrorCode = CustomErrorCode.Invalid,
                    CustomErrorMessage = "Invalid email and/or password",
                    CustomStackTrace = null
                };
            }

            return userLoginResponse;
        }

        [HttpPost]
        [Route("Update")]
        public Result Update([FromBody] UsersDTOs.Users user)
        {
            if (!UsersValidation.ValidateUpdate(user))
            {
                logger.Log(LogLevel.ERROR, "UsersValidation.ValidateUpdate Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("UsersValidation.ValidateUpdate Falied");
            }

            if (usersRepo.UserExists(user.EmailId, user.Id))
            {
                logger.Log(LogLevel.ERROR, "User with same emailId already exists");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Duplicate, CustomErrorMessage = "User with same emailId already exists" };
                //throw new Exception("User with same emailId already exists");
            }
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
