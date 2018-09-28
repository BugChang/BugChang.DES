using System;
using System.Collections.Generic;
using System.Linq;
using BugChang.DES.Core.Authorization.Menus;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace BugChang.DES.EntityFrameWorkCore
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<DesDbContext>();


                dbContext.Database.EnsureDeleted();

                dbContext.Database.EnsureCreated();

                if (!dbContext.Users.Any())
                {
                    #region Department

                    var department = new Department
                    {
                        Code = "001",
                        FullName = "初始单位",
                        Name = "初始单位"
                    };

                    dbContext.Departments.Add(department);

                    #endregion

                    #region User

                    var sysAdmin = new User
                    {
                        UserName = "sysadmin",
                        DisplayName = "系统管理员",
                        Enabled = true,
                        LoginErrorCount = 0,
                        Password = HashHelper.Md5(User.DefaultPassword),
                        Department = department,
                        UserRoles = new List<UserRole>
                        {
                            new UserRole
                            {
                                Role = new Role
                                {
                                    Name = Role.SysAdmin
                                }
                            }
                        }
                    };
                    dbContext.Users.Add(sysAdmin);

                    var secAdmin = new User
                    {
                        UserName = "secadmin",
                        DisplayName = "安全管理员",
                        Enabled = true,
                        LoginErrorCount = 0,
                        Password = HashHelper.Md5(User.DefaultPassword),
                        Department = department,
                        UserRoles = new List<UserRole>
                        {
                            new UserRole
                            {
                                Role = new Role
                                {
                                    Name = Role.SecAdmin
                                }
                            }
                        }
                    };
                    dbContext.Users.Add(secAdmin);

                    var audAdmin = new User
                    {
                        UserName = "audadmin",
                        DisplayName = "审计管理员",
                        Enabled = true,
                        LoginErrorCount = 0,
                        Password = HashHelper.Md5(User.DefaultPassword),
                        Department = department,
                        UserRoles = new List<UserRole>
                        {
                            new UserRole
                            {
                                Role = new Role
                                {
                                    Name = Role.AudAdmin
                                }
                            }
                        }
                    };
                    dbContext.Users.Add(audAdmin);

                    #endregion

                    #region Menu
                    var index = new Menu
                    {
                        Name = "首页",
                        Url = "/Home/Index"
                    };
                    var quanxian = new Menu
                    {
                        Name = "权限管理",
                        Items = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "用户管理",
                                Url = "/User/Index"
                            },
                            new Menu
                            {
                                Name = "组织机构",
                                Url = "/Department/Index"
                            },
                            new Menu
                            {
                                Name = "菜单管理",
                                Url = "/Menu/Index"
                            },
                            new Menu
                            {
                                Name = "角色管理",
                                Url = "/Role/Index"
                            }
                            ,new Menu
                            {
                                Name = "证卡管理",
                                Url = "/Card/Index"
                            }

                        }
                    };
                    var xitong = new Menu
                    {
                        Name = "系统管理",
                        Items = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "流转对象管理",
                                Url = "/ExchangeObject/Index"
                            },
                            new Menu
                            {
                                Name = "系统日志",
                                Url = "/Log/System"
                            },
                            new Menu
                            {
                                Name = "交换场所管理",
                                Url = "/Place/Index"
                            },
                            new Menu
                            {
                                Name = "箱格管理",
                                Url = "/Box/Index"
                            }
                            ,new Menu
                            {
                                Name = "客户端管理",
                                Url = "/Client/Index"
                            }
                            ,new Menu
                            {
                                Name = "单位分组",
                                Url = "/Group/Index"
                            }
                            ,new Menu
                            {
                                Name = "硬件设置",
                                Url = "/HardWare/Index"
                            }
                            ,new Menu
                            {
                                Name = "数据库备份",
                                Url = "/BackUp/Index"
                            }

                        }
                    };
                    var shenji = new Menu
                    {
                        Name = "审计查询",
                        Items = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "审计日志",
                                Url = "/Log/Audit"
                            }
                        }
                    };
                    var geren = new Menu
                    {
                        Name = "个人设置",
                        Items = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "修改密码",
                                Url = "/Account/ChangePassword"
                            }
                        }
                    };
                    var zonghe = new Menu
                    {
                        Name = "综合管理",
                        Items = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "清单打印",
                                Url = "/Bill/Index"
                            },
                            new Menu
                            {
                                Name = "历史清单",
                                Url = "/Bill/List"
                            }
                        }
                    };
                    var xinjian = new Menu
                    {
                        Name = "信件管理",
                        Items = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "收信登记",
                                Url = "/Letter/Receive"
                            },
                            new Menu
                            {
                                Name = "收信查询",
                                Url = "/Letter/ReceiveList"
                            },
                            new Menu
                            {
                                Name = "发信登记",
                                Url = "/Letter/Send"
                            },
                            new Menu
                            {
                                Name = "发信查询",
                                Url = "/Letter/SendList"
                            },
                            new Menu
                            {
                                Name = "信件退回",
                                Url = "/Letter/Back"
                            },
                            new Menu
                            {
                                Name = "信件勘误",
                                Url = "/Letter/Cancel"
                            },
                            new Menu
                            {
                                Name = "异形件管理",
                                Url = "/Letter/Different"
                            },
                            new Menu
                            {
                                Name = "信件分拣",
                                Url = "/Letter/Sorting"
                            },
                            new Menu
                            {
                                Name = "信件统计",
                                Url = "/Letter/Statistics"
                            },
                            new Menu
                            {
                                Name = "外收转内发查询",
                                Url = "/Letter/Out2Inside"
                            },
                        }
                    };

                    dbContext.Menus.Add(index);
                    dbContext.Menus.Add(quanxian);
                    dbContext.Menus.Add(xitong);
                    dbContext.Menus.Add(shenji);
                    dbContext.Menus.Add(geren);
                    dbContext.Menus.Add(geren);
                    dbContext.Menus.Add(zonghe);
                    dbContext.Menus.Add(xinjian);

                    #endregion

                    dbContext.SaveChanges();
                }
            }
        }
    }
}
