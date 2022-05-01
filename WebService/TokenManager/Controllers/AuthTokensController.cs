using AuthDTOs;
using CommonDTOs;
using CommonUtils.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceContracts;
using ServiceContracts.Logger;
using ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UsersDTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TokenManager.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTokensController : ControllerBase
    {
        private readonly AuthSettings authSettings;
        //private readonly IConfiguration config;
        private readonly IUsersRepository usersRepo;
        private readonly ITokensRepository tokensRepo;
        private readonly ILogger logger;

        public AuthTokensController(IConfiguration config, IUsersRepository usersRepo, ITokensRepository tokensRepo
            , ILogger logger)
        {
            authSettings = new AuthSettings();
            config.GetSection("AuthSettings").Bind(authSettings);
            //this.config = config;
            this.usersRepo = usersRepo;
            this.tokensRepo = tokensRepo;
            this.logger = logger;
        }

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "AuthTokensController -> Pong";
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAuthToken([FromBody] AuthRequest authRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { IsSuccess = false, Reason = "UserName and Password must be provided." });
            var userRequest = new UsersDTOs.Users()
            {
                EmailId = authRequest.UserName,
                Password = authRequest.Password
            };
            var user = usersRepo.ValidateLogin(userRequest);
            if (user == null)
                return Unauthorized();

            List<string> roles = new List<string>();
            if(user.IsSuperAdmin)
            {
                roles.Add("admin");
            }
            string remoteIPAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            string token = string.Empty;
            string refreshToken = string.Empty;
            AuthTokenHelper.GetToken(user.EmailId, roles, authSettings.Key, out token, out refreshToken);
            if(string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized();
            //invalidate previous tokens
            var userRefreshTokenRequest = new UserRefreshTokens
            {
                IpAddress = remoteIPAddress,
                IsInvalidated = false,
                RefreshToken = refreshToken,
                Token = token
            };
            var prevRefreshTokens = tokensRepo.GetRefreshTokenByFilterCondition(userRefreshTokenRequest);
            if (prevRefreshTokens != null && prevRefreshTokens.Count() > 0)
            {
                foreach (var prevToken in prevRefreshTokens)
                {
                    if (prevToken != null && prevToken.Id > 0)
                    {
                        tokensRepo.InvalidateRefreshToken(prevToken.Id);
                    }
                }
            }
            //add new refresh token row
            var userRefreshToken = new UserRefreshTokens
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                IpAddress = remoteIPAddress,
                IsInvalidated = false,
                RefreshToken = refreshToken,
                Token = token,
                UserId = user.Id
            };
            long refreshId = tokensRepo.AddRefreshToken(userRefreshToken);
            var authResponse = new AuthResponse { Token = token, RefreshToken = refreshToken, IsSuccess = true };
            return Ok(authResponse);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { IsSuccess = false, Reason = "Tokens must be provided" });
            string remoteIPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var securityToken = GetJwtToken(request.ExpiredToken);

            //invalidate previous tokens
            var userRefreshTokenRequest = new UserRefreshTokens
            {
                IpAddress = remoteIPAddress,
                IsInvalidated = false,
                RefreshToken = request.RefreshToken,
                Token = request.ExpiredToken
            };
            var userRefreshTokens = tokensRepo.GetRefreshTokenByFilterCondition(userRefreshTokenRequest);
            if (userRefreshTokens != null && userRefreshTokens.Count() > 0 && userRefreshTokens.FirstOrDefault() != null)
            {
                var userRefreshToken = userRefreshTokens.FirstOrDefault();
                AuthResponse response = ValidateDetails(securityToken, userRefreshToken);
                if (!response.IsSuccess)
                {
                    tokensRepo.InvalidateRefreshToken(userRefreshToken.Id);
                    return Unauthorized();
                }
                //invalidate previous token
                tokensRepo.InvalidateRefreshToken(userRefreshToken.Id);

                //create new token and put in db
                if (securityToken.Claims != null && securityToken.Claims.Count() > 0)
                {
                    var emailId = string.Empty;
                    var roles = new List<string>();
                    foreach (var claim in securityToken.Claims)
                    {
                        if (!string.IsNullOrWhiteSpace(claim.Value))
                        {
                            if (claim.Type == ClaimTypes.NameIdentifier)
                                emailId = claim.Value;
                            else if (claim.Type == ClaimTypes.Role )
                            {
                                roles = JsonConvert.DeserializeObject<List<string>>(claim.Value);
                            }
                        }
                    }
                    string token = string.Empty;
                    string refreshToken = string.Empty;
                    AuthTokenHelper.GetToken(emailId, roles, authSettings.Key, out token, out refreshToken);
                    if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
                        return Unauthorized();

                    //add new refresh token
                    var userRefreshTokenDetails = new UserRefreshTokens
                    {
                        CreatedDate = DateTime.UtcNow,
                        ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                        IpAddress = remoteIPAddress,
                        IsInvalidated = false,
                        //RefreshToken = refreshToken,
                        RefreshToken = request.RefreshToken,
                        Token = token,
                        UserId = userRefreshToken.UserId
                    };
                    long refreshId = tokensRepo.AddRefreshToken(userRefreshTokenDetails);
                    var authResponse = new AuthResponse { IsSuccess = true, Token = token, RefreshToken = request.RefreshToken };
                    return Ok(authResponse);
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("[action]")]
        public bool ValidateRefreshToken([FromBody] RefreshTokenRequest request)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                string remoteIPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                var securityToken = GetJwtToken(request.ExpiredToken);

                var userRefreshTokenRequest = new UserRefreshTokens
                {
                    IpAddress = remoteIPAddress,
                    IsInvalidated = false,
                    RefreshToken = request.RefreshToken,
                    Token = request.ExpiredToken
                };
                var userRefreshTokens = tokensRepo.GetRefreshTokenByFilterCondition(userRefreshTokenRequest);
                if (userRefreshTokens != null && userRefreshTokens.Count() > 0 && userRefreshTokens.FirstOrDefault() != null)
                {
                    var userRefreshToken = userRefreshTokens.FirstOrDefault();
                    AuthResponse response = ValidateDetails(securityToken, userRefreshToken);
                    result = response.IsSuccess;
                }
            }
            return result;
        }

        [NonAction]
        private JwtSecurityToken GetJwtToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(token);
        }

        [NonAction]
        private AuthResponse ValidateDetails(JwtSecurityToken securityToken, UserRefreshTokens userRefreshToken)
        {
            if (userRefreshToken == null)
                return new AuthResponse { IsSuccess = false, Reason = "Invalid Token Details." };
            //if (securityToken.ValidTo > DateTime.UtcNow)
            //    return new AuthResponse { IsSuccess = false, Reason = "Token not expired." };
            if (!userRefreshToken.IsActive)
                return new AuthResponse { IsSuccess = false, Reason = "Refresh Token Expired" };
            return new AuthResponse { IsSuccess = true };
        }
    }
}
