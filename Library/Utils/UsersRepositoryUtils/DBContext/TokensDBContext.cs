using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UsersDTOs;

namespace UsersRepositoryUtils.DBContext
{
    public class TokensDBContext : DbContext
    {
        public TokensDBContext(DbContextOptions<TokensDBContext> options) : base(options)
        {
        }
        public DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRefreshTokens>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_UserRefreshTokensId");
            modelBuilder.Entity<UserRefreshTokens>()
                    .Property(t => t.Token)
                    .IsRequired();
            modelBuilder.Entity<UserRefreshTokens>()
                   .Property(t => t.RefreshToken)
                   .IsRequired();
            modelBuilder.Entity<UserRefreshTokens>()
                   .Property(t => t.ExpirationDate)
                   .IsRequired();
            //modelBuilder.Entity<UserRefreshTokens>()
            //       .Property(t => t.IsActive)
            //       .IsRequired();
            modelBuilder.Entity<UserRefreshTokens>()
                   .Property(t => t.IpAddress)
                   .IsRequired();
            modelBuilder.Entity<UserRefreshTokens>()
                   .Property(t => t.IsInvalidated)
                   .IsRequired();
            modelBuilder.Entity<UserRefreshTokens>()
                   .Property(t => t.UserId)
                   .IsRequired();
        }
    }
}
