using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DataLayer.EfClasses;

namespace DataLayer.EfCode
{
    public class PwContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Operation> Operations { get; set; }

        public PwContext(DbContextOptions<PwContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operation>().HasKey(x => new { x.TransferId, x.UserId });
        }
    }
}
