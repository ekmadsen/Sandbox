using System;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class ServiceCall
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Technician Technician { get; set; }
        public DateTime Scheduled { get; set; }
        public bool Open { get; set; }
    }
}
