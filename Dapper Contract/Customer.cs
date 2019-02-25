using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public Technicians Technicians { get; set; }
        public ServiceCalls ServiceCalls { get; set; }


        public Customer()
        {
            Technicians = new Technicians();
            ServiceCalls = new ServiceCalls();
        }
    }
}
