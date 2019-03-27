using System.Collections.Generic;
using ErikTheCoder.ServiceContract;


namespace ErikTheCoder.Sandbox.AsyncConcurrent
{
    public class PcReport
    {
        public string ComputerName { get; set; }
        public int Days { get; set; }
        public User PrimaryUser { get; set; }
        public List<string> FileLogs { get; set; }
        public List<string> EventLogs { get; set; }
        public List<string> DatabaseLogs { get; set; }
    }
}
