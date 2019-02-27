using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class GetOpenServiceCallsResponse
    {
        public ServiceCalls ServiceCalls { get; [UsedImplicitly] set; }
        public Customers Customers { get; [UsedImplicitly] set; }
        public Technicians Technicians { get; [UsedImplicitly] set; }


        public GetOpenServiceCallsResponse()
        {
            ServiceCalls = new ServiceCalls();
            Customers = new Customers();
            Technicians = new Technicians();
        }
    }
}
