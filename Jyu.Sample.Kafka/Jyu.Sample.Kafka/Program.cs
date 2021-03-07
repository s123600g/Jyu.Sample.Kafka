using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Threading;

namespace Jyu.Sample.Kafka
{
    internal class Program
    {
        private static ProducerConfig kafkaConf;
        private static Logger logger;
        private static IConfiguration appConfig;
        private static KafkasettingsModel getKafkasettings;

        private static void Main(string[] args)
        {
            // 載入appsettings json 內容
            appConfig = InitializeSettings.AppsetingsLoad();
            // 從組態設定檔載入NLog設定
            logger = InitializeSettings.NLogInitialize(appConfig);
            // 載入Kafka設定
            getKafkasettings = appConfig.GetSection("Kafkasettings").Get<KafkasettingsModel>();
            // 載入Kafka產生者連線配置
            kafkaConf = new ProducerConfig { BootstrapServers = getKafkasettings.BootstrapServers };

            // 進行Topic內容產生
            Producer();

            // 訂閱指定Topic內容
            //Consumer();
        }

        /// <summary>
        /// Topic 內容產生者
        /// https://github.com/confluentinc/confluent-kafka-dotnet#basic-producer-examples
        /// </summary>
        private static void Producer()
        {
            // 官方範例是用Console.WriteLine，這裡改寫用NLog
            // 建立推送訊息產生事件CallBack，針對成功與失敗個別輸出各自訊息
            Action<DeliveryReport<Null, string>> handler = r =>
            logger.Info(string.Format("{0}",
            !r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}"

            ));

            using (var p = new ProducerBuilder<Null, string>(kafkaConf).Build())
            {
                // 發送訊息
                for (int i = 0; i < 100; ++i)
                {
                    p.Produce(
                        getKafkasettings.Topics.Topic_One, // Topic Name ，這裡要指定自己要送給哪一個Topic
                        new Message<Null, string> { Value = i.ToString() },  // 發送訊息內容
                        handler // CallBack
                    );
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                // 這裡是指定等待時間讓上面所有訊息都被成功發送出去
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Topic 內容訂閱者
        /// https://github.com/confluentinc/confluent-kafka-dotnet#basic-consumer-example
        /// </summary>
        private static void Consumer()
        {
            // 建立訂閱者資訊
            ConsumerConfig conf = new ConsumerConfig
            {
                // 訂閱者所屬Group Id
                GroupId = getKafkasettings.Groups.GroupId_One,
                // Kafka連線服務位址
                BootstrapServers = getKafkasettings.BootstrapServers,
                // 設置取得訊息時間點位置，預設是從最早建立時間點位置開始抓起
                // Earliest --> 從一開始最早時間(最舊，一開始初次建立那一筆訊息)
                // Latest --> 從最晚時間點開始(最新)
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                // 設置訂閱Topic
                c.Subscribe(getKafkasettings.Topics.Topic_One);

                // 建立CancellationToken
                // https://docs.microsoft.com/zh-tw/dotnet/api/system.threading.cancellationtokensource?view=net-5.0
                CancellationTokenSource cts = new CancellationTokenSource();

                // https://docs.microsoft.com/zh-tw/dotnet/api/system.console.cancelkeypress?view=net-5.0
                // 偵測是否有按下Ctrl+C
                Console.CancelKeyPress += (_, e) =>
                {
                    // 當使用者按下Ctrl+C時，不應該先直接結束整個應用程序
                    // 而是透過CancellationTokenSource內機制，告訴下方訂閱服務請求終止，進行完整程序結束流程
                    // 而Cancel此屬性設置為True是意謂著當言程序繼續進行著，反之則是終結程序進行
                    // prevent the process from terminating.
                    e.Cancel = true;

                    logger.Info(e.SpecialKey);

                    // 告訴程序訂閱服務請求終止
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            // 保持訂閱狀態，依據CancellationTokenSource內CancellationToken Token來進行訂閱狀態改變
                            var cr = c.Consume(cts.Token);
                            logger.Info($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            logger.Error($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    logger.Error("Application was terminated...");
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
    }
}