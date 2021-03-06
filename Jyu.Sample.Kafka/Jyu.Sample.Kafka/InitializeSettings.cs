using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

namespace Jyu.Sample.Kafka
{
    public class InitializeSettings
    {
        /// <summary>
        /// 載入組態設定檔
        /// </summary>
        public static IConfiguration AppsetingsLoad()
        {
            // 載入appsettings json 內容
            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("Kafkasettings.json", optional: true, reloadOnChange: true)
            .Build();

            return config;
        }

        /// <summary>
        /// NLog載入
        /// </summary>
        public static Logger NLogInitialize(IConfiguration config)
        {
            // NLog configuration with appsettings.json
            // https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-configuration-with-appsettings.json
            // 從組態設定檔載入NLog設定
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            Logger logger = LogManager.GetCurrentClassLogger();

            return logger;
        }
    }
}
