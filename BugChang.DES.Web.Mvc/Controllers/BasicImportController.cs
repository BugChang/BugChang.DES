using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Application.Users;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Exchanges.Channel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BasicImportController : Controller
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IUserAppService _userAppService;

        public BasicImportController(IHostingEnvironment hostingEnvironment, IDepartmentAppService departmentAppService, IUserAppService userAppService)
        {
            _hostingEnvironment = hostingEnvironment;
            _departmentAppService = departmentAppService;
            _userAppService = userAppService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Import(IFormFile excelfile)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            try
            {
                using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                {
                    excelfile.CopyTo(fs);
                    fs.Flush();
                }
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 3; row <= rowCount; row++)
                    {
                        int userDepartmentId = 0;
                        var department2 = new DepartmentEditDto
                        {
                            Name = worksheet.Cells[row, 1].Value.ToString(),
                            Code = worksheet.Cells[row, 2].Value.ToString(),
                            ParentId = 1,
                            ReceiveChannel = (int)EnumChannel.内部
                        };

                        var department2Id = await _departmentAppService.CheckForImport(1, department2.Code, department2.Name);
                        if (department2Id == 0)
                        {
                            await _departmentAppService.AddOrUpdateAsync(department2);
                            department2Id = await _departmentAppService.CheckForImport(1, department2.Code, department2.Name);
                        }
                        userDepartmentId = department2Id;
                        if (department2Id != 0 && worksheet.Cells[row, 3].Value != null)
                        {

                            var department3 = new DepartmentEditDto
                            {
                                Name = worksheet.Cells[row, 3].Value.ToString(),
                                Code = worksheet.Cells[row, 4].Value.ToString(),
                                ParentId = department2Id,
                                ReceiveChannel = (int)EnumChannel.内部
                            };
                            var department3Id = await _departmentAppService.CheckForImport(department2Id, department3.Code, department3.Name);
                            if (department3Id == 0)
                            {
                                await _departmentAppService.AddOrUpdateAsync(department3);
                                department3Id = await _departmentAppService.CheckForImport(department2Id, department3.Code, department3.Name);
                            }
                            userDepartmentId = department3Id;
                            if (department3Id != 0 && worksheet.Cells[row, 5].Value != null)
                            {
                                var department4 = new DepartmentEditDto
                                {
                                    Name = worksheet.Cells[row, 5].Value.ToString(),
                                    Code = worksheet.Cells[row, 6].Value.ToString(),
                                    ParentId = department3Id,
                                    ReceiveChannel = (int)EnumChannel.内部
                                };
                                var department4Id = await _departmentAppService.CheckForImport(department3Id, department4.Code, department4.Name);
                                if (department4Id == 0)
                                {
                                    await _departmentAppService.AddOrUpdateAsync(department4);
                                    department4Id = await _departmentAppService.CheckForImport(department3Id, department4.Code, department4.Name);
                                }

                                userDepartmentId = department4Id;
                            }

                            var user = new UserEditDto
                            {
                                DepartmentId = userDepartmentId,
                                CreateTime = DateTime.Now,
                                DisplayName = worksheet.Cells[row, 8].Value.ToString(),
                                UserName = worksheet.Cells[row, 7].Value.ToString(),
                                Enabled = 1
                            };
                            await _userAppService.AddOrUpdateAsync(user);
                        }


                        sb.Append(Environment.NewLine);
                    }
                    return Content(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }


}
