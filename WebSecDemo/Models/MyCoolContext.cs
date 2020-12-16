using Microsoft.EntityFrameworkCore;

namespace WebSecDemo.Models
{
    public class MyCoolContext : DbContext
    {
        public MyCoolContext()
        {
            
        }
        public MyCoolContext(DbContextOptions<MyCoolContext> options)
            :base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=localhost;initial catalog=websec;integrated security=true");
            }
        }


        public DbSet<UserAccount> Accounts { get; set; }
        public DbSet<UserAccountHashed> HashedAccounts { get; set; }
    }
}