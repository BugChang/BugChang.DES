using System;
using System.Linq;
using System.Linq.Expressions;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Logs;
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
            modelBuilder.Entity<Department>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<Department>().HasOne(a => a.UpdateUser);

            //全局过滤器
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(ISoftDelete).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType).Property<Boolean>("IsDeleted");
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant("IsDeleted")),
                    Expression.Constant(false));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Power> Powers { get; set; }

        public DbSet<RoleMenu> RoleMenus { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<RoleOperation> RoleOperations { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Place> Places { get; set; }

        public DbSet<Box> Boxs { get; set; }

        public DbSet<ExchangeObject> ExchangeObjects { get; set; }

        public DbSet<BoxObject> BoxObjects { get; set; }

    }
}
