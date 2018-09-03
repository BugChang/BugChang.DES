using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.BackUps.Dtos;
using BugChang.DES.Core.BackUps;
using BugChang.DES.Core.Commons;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BugChang.DES.Application.BackUps
{
    public class BackUpAppService : IBackUpAppService
    {
        private readonly IBackUpRepository _backUpRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IOptions<BackUpSettings> _backUpSettings;

        public BackUpAppService(IBackUpRepository backUpRepository, UnitOfWork unitOfWork, IOptions<BackUpSettings> backUpSettings)
        {
            _backUpRepository = backUpRepository;
            _unitOfWork = unitOfWork;
            _backUpSettings = backUpSettings;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(BackUpEditDto editDto)
        {
            var result = new ResultEntity();
            var entity = Mapper.Map<DataBaseBackUp>(editDto);
            await _backUpRepository.AddAsync(entity);
            result.Success = true;
            await _unitOfWork.CommitAsync();
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = new ResultEntity();
            var entity = await _backUpRepository.GetByIdAsync(id);
            entity.IsDeleted = true;
            await _unitOfWork.CommitAsync();
            result.Success = true;
            return result;
        }

        public Task<BackUpEditDto> GetForEditByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PageResultModel<BackUpListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            var queryable = _backUpRepository.GetQueryable();
            if (!string.IsNullOrWhiteSpace(pageSearchDto.Keywords))
            {
                queryable = queryable.Where(a => a.FileName.Contains(pageSearchDto.Keywords));
            }

            var backups = await queryable.OrderByDescending(a => a.Id).Skip(pageSearchDto.Skip).Take(pageSearchDto.Take)
                .ToListAsync();

            return new PageResultModel<BackUpListDto>
            {
                Rows = Mapper.Map<IList<BackUpListDto>>(backups),
                Total = await queryable.CountAsync()
            };
        }

        public async Task<ResultEntity> BackUpNow(int type, string operatorName, string remark)
        {
            var result = new ResultEntity();
            try
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmm");
                var cmdResult = RunCmd(string.Format(_backUpSettings.Value.BackUpScript, _backUpSettings.Value.BackUpPath + (type == 1 ? "Auto" : "Hand") + "\\", fileName));

                //获取备份文件的MD5并存入数据库
                var fileFullName = string.Format(_backUpSettings.Value.BackUpPath + "{0}\\bugchang.des_" + fileName + ".sql", type == 1 ? "Auto" : "Hand");
                var fileMd5 = GetMd5HashFromFile(fileFullName);
                var backup = new BackUpEditDto
                {
                    DateTime = DateTime.Now,
                    FileName = fileFullName,
                    Md5 = fileMd5,
                    OperatorName = operatorName,
                    Type = type,
                    Remark = remark
                };
                await AddOrUpdateAsync(backup);
                result.Success = true;
            }
            catch (Exception exception)
            {
                result.Message = exception.ToString();
                result.Success = false;
            }
            return result;
        }

        private static string GetMd5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        private static string RunCmd(string strInput)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.WriteLine(strInput);
            p.StandardInput.WriteLine("exit");
            return p.StandardError.ReadToEnd();
        }
    }
}
