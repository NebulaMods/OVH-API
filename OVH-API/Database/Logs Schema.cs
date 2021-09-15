using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace OVHAPI.Database
{
    public class LogsSchema
    {
        public class ErrorLogs
        {
            [Key]
            public int ID { get; set; }

            public string Application { get; set; }

            public string Name { get; set; }

            public string Location { get; set; }

            public string Reason { get; set; }

            public DateTime ErrorTime { get; set; }
        }

        public class AttackLogs
        {
            [Key]
            public int ID { get; set; }

            public string Server { get; set; }

            public IPAddress IPAttacked { get; set; }

            public string Duration { get; set; }

            public DateTime DetectionTime { get; set; }

            public DateTime EndingTime { get; set; }

            public bool Active { get; set; }

        }
    }
}
