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
    public class TokensRepository : ITokensRepository
    {
        private readonly TokensDBContext context;

        public TokensRepository(TokensDBContext context)
        {
            this.context = context;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public long AddRefreshToken(UserRefreshTokens userRefreshToken)
        {
            userRefreshToken.CreatedDate = DateTime.Now;
            context.UserRefreshTokens.Add(userRefreshToken);
            context.SaveChanges();
            long id = userRefreshToken.Id;
            return id;
        }

        public UserRefreshTokens GetRefreshTokenById(long id)
        {
            var userRefreshTokens = context.UserRefreshTokens.Include(x => x.User).FirstOrDefault(x => x.Id == id && !x.IsInvalidated);
            context.Entry(userRefreshTokens).State = EntityState.Detached;
            return userRefreshTokens;
        }

        public IEnumerable<UserRefreshTokens> GetRefreshTokenByFilterCondition(UserRefreshTokens userRefreshToken)
        {
            //var userRefreshTokens = new List<UserRefreshTokens>();
            //if (userRefreshToken == null)
            //{
            //    userRefreshTokens = context.UserRefreshTokens.Include(x => x.User).Where(x => !x.IsInvalidated).ToList();
            //}
            //else
            //{
            IEnumerable<UserRefreshTokens> userRefreshTokens = context.UserRefreshTokens.Include(x => x.User).Where(x => !x.IsInvalidated
                && (userRefreshToken.Id <= 0 || userRefreshToken.Id == x.Id)
                && (string.IsNullOrWhiteSpace(userRefreshToken.Token) || x.Token.Equals(userRefreshToken.Token))
                && (string.IsNullOrWhiteSpace(userRefreshToken.RefreshToken) || x.RefreshToken.Equals(userRefreshToken.RefreshToken))
                && (userRefreshToken.UserId <= 0 || userRefreshToken.UserId == x.UserId)).ToList();
            //}
            //context.Entry(userRefreshTokens).State = EntityState.Detached;
            return userRefreshTokens;
        }

        public bool InvalidateRefreshToken(long id)
        {
            bool result = true;
            if (id > 0 && context.UserRefreshTokens.Any(x => x.Id == id))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var userRefreshTokenExisting = context.UserRefreshTokens.First(x => x.Id == id);
                context.Entry(userRefreshTokenExisting).State = EntityState.Detached;
                userRefreshTokenExisting.IsInvalidated = true;

                var userRefreshTokenUpdated = context.UserRefreshTokens.Attach(userRefreshTokenExisting);
                userRefreshTokenUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool UpdateRefreshToken(UserRefreshTokens userRefreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
