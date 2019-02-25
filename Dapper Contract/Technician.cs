namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class Technician
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Customers Customers { get; set; }
        public ServiceCalls ServiceCalls { get; set; }



        public Technician()
        {
            Customers = new Customers();
            ServiceCalls = new ServiceCalls();
        }
    }
}
