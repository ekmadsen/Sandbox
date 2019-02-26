using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class Technician
    {
        public int Id { get; [UsedImplicitly] set; }
        [UsedImplicitly] public string Name { get; set; }
        public Customers Customers { get; [UsedImplicitly] set; }
        public ServiceCalls ServiceCalls { get; [UsedImplicitly] set; }



        public Technician()
        {
            Customers = new Customers();
            ServiceCalls = new ServiceCalls();
        }
    }
}
