using Core2Walkthrough.Data;
using Microsoft.EntityFrameworkCore;

/*
 *  About: Holds the middle man that maps the database tables to the datasets to move data to and from the database
 *  
 */
namespace Core2Walkthrough
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<USERS> USER_DBSet { get; set; }

        public DbSet<NEWS> NEWS_DBSet { get; set; }

        public DbSet<ACTIVITY> ACTIVITY_DBSet { get; set; }

        public DbSet<ACTIVITY_LOG> ACTIVITY_LOG_DBSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Important to db linking.. Table names _MUST_ be accurate or risk not being found.
            modelBuilder.Entity<USERS>().ToTable("USERS");
            modelBuilder.Entity<NEWS>().ToTable("NEWS");
            modelBuilder.Entity<ACTIVITY>().ToTable("ACTIVITY");
            modelBuilder.Entity<ACTIVITY_LOG>().ToTable("ACTIVITY_LOG");

        }
    }

}
