using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BugChang.DES.Core.Tools
{
    public static class ComputerInfoHelper
    {

        [DllImport("kernel32")]
        public static extern void GetSystemDirectory(StringBuilder sysDir, int count);
        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref CpuInfo cpuinfo);
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MemoryInfo meminfo);

        private static PerformanceCounter _cpu;
        ///<summary>
        /// 获取指定驱动器的空间总大小(单位为GB)  
        /// </summary>  
        /// <param name="strHardDiskName">只需输入代表驱动器的字母即可</param>
        /// <returns> </returns>  
        public static long GetHardDiskSpace(string strHardDiskName)
        {

            long totalSize = new long();

            strHardDiskName = strHardDiskName + ":\\";

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (System.IO.DriveInfo drive in drives)

            {

                if (drive.Name == strHardDiskName)

                {

                    totalSize = drive.TotalSize / (1024 * 1024 * 1024);

                }

            }

            return totalSize;

        }

        /// <summary>  
        /// 获取指定驱动器的剩余空间总大小(单位为GB)  
        /// </summary>  
        /// <param name="strHardDiskName">只需输入代表驱动器的字母即可</param>
        /// <returns> </returns>  
        public static long GetHardDiskFreeSpace(string strHardDiskName)

        {

            long freeSpace = new long();

            strHardDiskName = strHardDiskName + ":\\";

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (System.IO.DriveInfo drive in drives)

            {

                if (drive.Name == strHardDiskName)

                {

                    freeSpace = drive.TotalFreeSpace / (1024 * 1024 * 1024);

                }

            }

            return freeSpace;

        }

        public static long GetHardDiskUseSpace(string strHardDiskName)
        {
            return GetHardDiskSpace(strHardDiskName) - GetHardDiskFreeSpace(strHardDiskName);
        }

        /// <summary>
        /// 获取硬盘使用率
        /// </summary>
        /// <param name="strHardDiskName"></param>
        /// <returns></returns>
        public static int GetHardDiskUsageRate(string strHardDiskName)
        {
            return 100 - Convert.ToInt32((GetHardDiskFreeSpace(strHardDiskName) * 100) / GetHardDiskSpace(strHardDiskName));
        }




        public static int GetMemoryUsageRate()
        {

            // cif = new ComputerInfo();
            var memInfo = new MemoryInfo();
            GlobalMemoryStatus(ref memInfo);
            return (int)memInfo.dwMemoryLoad;


        }

        public static int GetCpuUsageRate()
        {
            _cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            var percentage = 0;
            while (percentage == 0)
            {
                percentage = (int)_cpu.NextValue();
                Thread.Sleep(1000);
            }

            return percentage;
        }

        //定义CPU的信息结构  
        [StructLayout(LayoutKind.Sequential)]
        public struct CpuInfo
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }
        //定义内存的信息结构  
        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryInfo
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public uint dwTotalPhys;
            public uint dwAvailPhys;
            public uint dwTotalPageFile;
            public uint dwAvailPageFile;
            public uint dwTotalVirtual;
            public uint dwAvailVirtual;
        }
    }
}
