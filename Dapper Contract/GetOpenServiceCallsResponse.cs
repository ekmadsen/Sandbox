namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class GetOpenServiceCallsResponse
    {
        public ServiceCalls ServiceCalls { get; set; }
        public Customers Customers { get; set; }
        public Technicians Technicians { get; set; }
    }
}
