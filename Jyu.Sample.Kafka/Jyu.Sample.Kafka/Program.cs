using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using Confluent.Kafka;

namespace Jyu.Sample.Kafka
{
    class Program
    {
        static void Main(string[] args)
        {
            // 載入appsettings json 內容
            IConfiguration config = InitializeSettings.AppsetingsLoad();
            // 從組態設定檔載入NLog設定
            Logger logger = InitializeSettings.NLogInitialize(config);

            KafkasettingsModel getKafkasettings = config.GetSection("Kafkasettings").Get<KafkasettingsModel>();

            logger.Info(getKafkasettings.BootstrapServers);
        }
    }
}
