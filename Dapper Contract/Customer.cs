using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class Customer
    {
        public int Id { get; [UsedImplicitly] set; }
        [UsedImplicitly] public string Name { get; set; }
        [UsedImplicitly] public string Address { get; set; }
        [UsedImplicitly] public string City { get; set; }
        [UsedImplicitly] public string State { get; set; }
        [UsedImplicitly] public string ZipCode { get; set; }
        public Technicians Technicians { get; [UsedImplicitly] set; }
        public ServiceCalls ServiceCalls { get; [UsedImplicitly] set; }


        public Customer()
        {
            Technicians = new Technicians();
            ServiceCalls = new ServiceCalls();
        }
    }
}
