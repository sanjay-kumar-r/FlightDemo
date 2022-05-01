using System;
using System.Collections.Generic;
using System.Text;
using UsersDTOs;

namespace ServiceContracts.Users
{
    public interface ITokensRepository
    {
        UserRefreshTokens GetRefreshTokenById(long id);

        IEnumerable<UserRefreshTokens> GetRefreshTokenByFilterCondition(UserRefreshTokens userRefreshToken);

        long AddRefreshToken(UserRefreshTokens userRefreshToken);

        bool UpdateRefreshToken(UserRefreshTokens userRefreshToken);

        bool InvalidateRefreshToken(long id);
    }
}
