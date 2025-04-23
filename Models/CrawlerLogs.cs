using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CrawlerLogs
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public string JsonPath { get; set; }
    }
}
