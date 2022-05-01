using AuthDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.Tokens
{
    public class AuthTokenHelper
    {
        public static void GetToken(string userName, List<string> roles, string authKey, out string token, out string refreshToken)
        {
            token = GenerateToken(userName, roles, authKey);
            refreshToken = GenerateRefreshToken();
        }

        private static string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray);
            }
        }


        private static string GenerateToken(string userName, List<string> roles, string authKey)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
            // Add roles as multiple claims
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var keyBytes = Encoding.ASCII.GetBytes(authKey);
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        //public async Task<bool> IsTokenValid(string accessToken, string ipAddress)
        //{
        //    var isValid = context.UserRefreshTokens.FirstOrDefault(x => x.Token == accessToken
        //    && x.IpAddress == ipAddress) != null;
        //    return await Task.FromResult(isValid);
        //}
    }
}
