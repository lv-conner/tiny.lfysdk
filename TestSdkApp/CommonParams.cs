﻿namespace TestSdkApp // Note: actual namespace depends on the project name.
{
    public class CommonParams
    {
        /// <summary>
        /// 在平台申请的APPID信息
        /// </summary>
        public string app_id { get; set; } = "";

        /// <summary>
        /// 请求用户服务返回的uid，用户及设备级别个性化功能依赖此参数
        /// </summary>
        public string uid { get; set; } = "";
    }

}