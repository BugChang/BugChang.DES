namespace BugChang.DES.Application.Monitor.Dtos
{
    public class BoxGroupListDto
    {
        /// <summary>
        /// 箱组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 前扫描头BN号
        /// </summary>
        public string FrontScanBn { get; set; }

        /// <summary>
        /// 前多功能屏BN号
        /// </summary>
        public string FrontShowBn { get; set; }

        /// <summary>
        /// 前语音卡BN号
        /// </summary>
        public string FrontSoundBn { get; set; }

        /// <summary>
        /// 前读卡器BN号
        /// </summary>
        public string FrontReadCardBn { get; set; }

        /// <summary>
        /// 前摄像头BN号
        /// </summary>
        public string FrontCameraBn { get; set; }

        /// <summary>
        /// 前指静脉BN号
        /// </summary>
        public string FrontDigitalVein { get; set; }

        /// <summary>
        /// 后读卡器BN号
        /// </summary>
        public string BackReadCardBn { get; set; }

        /// <summary>
        /// 后扫描头BN号
        /// </summary>
        public string BackScanBn { get; set; }

        /// <summary>
        /// 后摄像头BN号
        /// </summary>
        public string BackCameraBn { get; set; }

        /// <summary>
        /// 后多功能屏BN号
        /// </summary>
        public string BackShowBn { get; set; }

        /// <summary>
        /// 后语音卡BN号
        /// </summary>
        public string BackSoundBn { get; set; }

        /// <summary>
        /// 后指静脉BN号
        /// </summary>
        public string BackDigitalVein { get; set; }

        /// <summary>
        /// 包含箱子
        /// 箱子ID逗号分隔，如：1，2，3
        /// </summary>
        public string Boxs { get; set; }
    }
}
