using BugChang.DES.Core.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BugChang.DES.EntityFrameWorkCore
{
    public class BasicDdContext : DbContext, IDbContext
    {
        public BasicDdContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;database=BugChang.DES.Basic;uid=root;pwd=19941114;");

        }

        public DbSet<User> Users { get; set; }
    }
}
