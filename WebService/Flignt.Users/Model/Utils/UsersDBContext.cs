using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Users.Model.Utils
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
        {
        }

        public DbSet<UserDtOs.Users> Users { get; set; }
        public DbSet<UserDtOs.AccountStatus> AccountStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDtOs.AccountStatus>()
                .HasKey(a => a.Id)
                .HasName("PrimaryKey_AccStatusId");
            modelBuilder.Entity<UserDtOs.AccountStatus>()
                   .Property(t => t.Status)
                   .IsRequired();

            PopulateAccountStatus(ref modelBuilder);

            modelBuilder.Entity<UserDtOs.Users>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_UserId");
            modelBuilder.Entity<UserDtOs.Users>()
                    .Property(t => t.FirstName)
                    .IsRequired();
            modelBuilder.Entity<UserDtOs.Users>()
                   .Property(t => t.LastName)
                   .IsRequired();
            modelBuilder.Entity<UserDtOs.Users>()
                   .Property(t => t.EmailId)
                   .IsRequired();
            modelBuilder.Entity<UserDtOs.Users>()
                   .Property(t => t.Password)
                   .IsRequired();
           modelBuilder.Entity<UserDtOs.Users>()
                  .Property(t => t.AccountStatusId)
                  .IsRequired();
           // modelBuilder.Entity<UserDtOs.Users>()
           // .HasRequired<UserDtOs.AccountStatus>(s => s.)
           // .WithMany(g => g.Students)
           // .HasForeignKey<int>(s => s.CurrentGradeId);
        }

        protected void PopulateAccountStatus(ref ModelBuilder modelBuilder)
        {
            List<UserDtOs.AccountStatus> accountStatus = new List<UserDtOs.AccountStatus>();
            foreach (var status in Enum.GetNames(typeof(UserDtOs.AccountStatusCode)))
            {
                accountStatus.Add(new UserDtOs.AccountStatus()
                {
                    Id = (int)Enum.Parse(typeof(UserDtOs.AccountStatusCode), status),
                    Status = status,
                    Description = UserDtOs.AccountStatusDescription.accountStatusDescriptions[status.ToUpper()]
                });
            }
            modelBuilder.Entity<UserDtOs.AccountStatus>().HasData(accountStatus);
        }
    }
}
