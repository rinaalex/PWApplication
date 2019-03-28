using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLayer.EfCode
{
    /// <summary>
    /// This class is needed to allow Add-Migration command to be run
    /// </summary>
    public class ContextFactory : IDesignTimeDbContextFactory<PwContext>
    {
        private const string connectionString =
            @"Server=LAPTOP-760ETF1K\SQLEXPRESS;Database=PwAppDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        public PwContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PwContext>();
            optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("DataLayer"));
            return new PwContext(optionsBuilder.Options);
        }
    }
}
