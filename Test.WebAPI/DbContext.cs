
using Microsoft.EntityFrameworkCore;
using Test.WebAPI.Modles;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Test.WebAPI
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options: options)
        {
        }
        public DbSet<Entity> EntityBase { get; set; }
        public DbSet<People> Peoples { get; set; }
    }
}
