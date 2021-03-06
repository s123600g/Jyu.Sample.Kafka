using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyu.Sample.Kafka
{
    public class KafkasettingsModel
    {
        public string BootstrapServers { get; set; }
        public Topics Topics { get; set; }
        public Groups Groups { get; set; }
    }

    public class Topics
    {
        public string Topic_One { get; set; }
    }

    public class Groups
    {
        public string GroupId_One { get; set; }
    }
}
