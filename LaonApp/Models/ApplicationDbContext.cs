using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //Add Model
        public DbSet<Account> Account { get; set; }
        public DbSet<Auth> Auth { get; set; }
        public DbSet<ConfirmEmail> ConfirmEmail { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<PasswordReset> PasswordReset { get; set; }


    }
}
