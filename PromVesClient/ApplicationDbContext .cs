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
        public DbSet<Wagon> Wagons => Set<Wagon>();
        //настрйока каскадного удаления
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Weighing>()
                .HasOne(w => w.Receipt)
                .WithMany(r => r.Weighings)
                .HasForeignKey(w => w.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
