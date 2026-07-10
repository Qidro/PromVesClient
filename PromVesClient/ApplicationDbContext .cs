using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using PromVesClient.Models;
namespace PromVesClient
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<Weighing> Weighings => Set<Weighing>();
    }
}
