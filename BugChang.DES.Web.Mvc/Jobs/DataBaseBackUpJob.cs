using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using BugChang.DES.Core.BackUps;
using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;

namespace BugChang.DES.Web.Mvc.Jobs
{
    public class DataBaseBackUpJob : Job
    {
        private readonly IOptions<BackUpSettings> _backUpSettings;
        public DataBaseBackUpJob(IOptions<BackUpSettings> backUpSettings)
        {
            _backUpSettings = backUpSettings;
        }

        // Begin 起始时间；Interval执行时间间隔，单位是毫秒，建议使用以下格式，此处为3小时；
        //SkipWhileExecuting是否等待上一个执行完成，true为等待；
        [Invoke(Begin = "2018-09-01 02:00", Interval = 1000 * 3600 * 24, SkipWhileExecuting = true)]
        public void Run()
        {
            //Job要执行的逻辑代码
            //备份数据库
            var fileName = DateTime.Now.ToString("yyyyMMddHHmm");
            RunCmd(string.Format(_backUpSettings.Value.BackUpScript, _backUpSettings.Value.BackUpPath + "//Auto//", fileName));

            //获取备份文件的MD5并存入数据库
            var fileFullName = _backUpSettings.Value.BackUpPath + "//Auto//bugchang.des_" + fileName + ".sql";
            var fileMd5 = GetMd5HashFromFile(fileFullName);

            //将文件重命名并移动至归档备份
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
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",//设置要启动的应用程序
                    UseShellExecute = false, //是否使用操作系统shell启动
                    RedirectStandardInput = true,//接受来自调用程序的输入信息
                    RedirectStandardOutput = true, //输出信息
                    RedirectStandardError = true,// 输出错误
                    CreateNoWindow = true //不显示程序窗口
                }
            };
            //启动程序
            p.Start();
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(strInput + "&exit");
            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();
            return strOuput;
        }

    }
}
