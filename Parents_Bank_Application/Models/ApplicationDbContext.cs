using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Parents_Bank_Application.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Bank_Account> Bank_Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<WishList_Item> WishList_Items { get; set; }

        public static ApplicationDbContext Create()
        {

            return new ApplicationDbContext();
        }
    }
}