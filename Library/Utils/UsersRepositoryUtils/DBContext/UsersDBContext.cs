using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace UsersRepositoryUtils.DBContext
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
        {
        }

        public DbSet<UsersDTOs.Users> Users { get; set; }
        public DbSet<UsersDTOs.AccountStatus> AccountStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersDTOs.AccountStatus>()
                .HasKey(a => a.Id)
                .HasName("PrimaryKey_AccStatusId");
            modelBuilder.Entity<UsersDTOs.AccountStatus>()
                   .Property(t => t.Status)
                   .IsRequired();

            PopulateAccountStatus(ref modelBuilder);

            modelBuilder.Entity<UsersDTOs.Users>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_UserId");
            modelBuilder.Entity<UsersDTOs.Users>()
                    .Property(t => t.FirstName)
                    .IsRequired();
            modelBuilder.Entity<UsersDTOs.Users>()
                   .Property(t => t.LastName)
                   .IsRequired();
            modelBuilder.Entity<UsersDTOs.Users>()
                   .Property(t => t.EmailId)
                   .IsRequired();
            modelBuilder.Entity<UsersDTOs.Users>()
                   .Property(t => t.Password)
                   .IsRequired();
            modelBuilder.Entity<UsersDTOs.Users>()
                   .Property(t => t.AccountStatusId)
                   .IsRequired();
            // modelBuilder.Entity<UserDtOs.Users>()
            // .HasRequired<UserDtOs.AccountStatus>(s => s.)
            // .WithMany(g => g.Students)
            // .HasForeignKey<int>(s => s.CurrentGradeId);
        }

        protected void PopulateAccountStatus(ref ModelBuilder modelBuilder)
        {
            List<UsersDTOs.AccountStatus> accountStatus = new List<UsersDTOs.AccountStatus>();
            foreach (var status in Enum.GetNames(typeof(UsersDTOs.AccountStatusCode)))
            {
                accountStatus.Add(new UsersDTOs.AccountStatus()
                {
                    Id = (int)Enum.Parse(typeof(UsersDTOs.AccountStatusCode), status),
                    Status = status,
                    Description = UsersDTOs.AccountStatusDescription.accountStatusDescriptions[status.ToUpper()]
                });
            }
            modelBuilder.Entity<UsersDTOs.AccountStatus>().HasData(accountStatus);
        }
    }
}
