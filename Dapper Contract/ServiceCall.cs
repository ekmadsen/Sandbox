using System;
using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class ServiceCall
    {
        public int Id { get; [UsedImplicitly] set; }
        [UsedImplicitly] public Customer Customer { get; set; }
        [UsedImplicitly] public Technician Technician { get; set; }
        [UsedImplicitly] public DateTime Scheduled { get; set; }
        [UsedImplicitly] public bool Open { get; set; }
    }
}
