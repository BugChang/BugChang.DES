using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;
using BugChang.DES.Infrastructure.Encryption;
using Microsoft.Extensions.DependencyInjection;

namespace BugChang.DES.EntityFrameWorkCore
{
    public static class SeedData
    {
        public static void InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<MainDbContext>();

                //dbContext.Database.EnsureDeleted();

                dbContext.Database.EnsureCreated();

                if (!dbContext.Users.Any())
                {
                    var user = new User
                    {
                        UserName = "bugchang",
                        DisplayName = "BugChang",
                        Enabled = true,
                        LoginErrorCount = 0,
                        Password = HashHelper.Md5("000000..")
                    };

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                }
            }

            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<LogDbContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}
