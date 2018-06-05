using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Departments;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore
{
    public class DesDbContext : DbContext
    {
        public DesDbContext(DbContextOptions<DesDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasOne(a => a.User).WithMany(a => a.UserRoles);
            modelBuilder.Entity<Department>().HasMany(a => a.Users).WithOne(a => a.Department);
            modelBuilder.Entity<User>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<User>().HasOne(a => a.UpdateUser);
            modelBuilder.Entity<UserRole>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<UserRole>().HasOne(a => a.UpdateUser);
            modelBuilder.Entity<Department>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<Department>().HasOne(a => a.UpdateUser);

            //全局过滤器
            modelBuilder.Entity<Department>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(p => !p.IsDeleted);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Power> Powers { get; set; }

        public DbSet<RoleMenu> RoleMenus { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<Department> Departments { get; set; }

    }
}
