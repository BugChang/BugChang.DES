﻿using System;
using System.Linq;
using System.Reflection;
using BugChang.DES.Core;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Powers;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Departments;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasOne(a => a.User).WithMany(a => a.UserRoles);
            modelBuilder.Entity<Department>().HasMany(a => a.Users).WithOne(a => a.Department);

            //这么写所有的字段都生成在baseentity表
            //modelBuilder.Entity<BaseEntity>().HasOne(a => a.CreateUser);

            //这么写太多了
            modelBuilder.Entity<User>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<User>().HasOne(a => a.UpdateUser);
            modelBuilder.Entity<UserRole>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<UserRole>().HasOne(a => a.UpdateUser);
            modelBuilder.Entity<Department>().HasOne(a => a.CreateUser);
            modelBuilder.Entity<Department>().HasOne(a => a.UpdateUser);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Power> Powers { get; set; }

        public DbSet<RolePower> RolePowers { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<Department> Departments { get; set; }

    }
}
