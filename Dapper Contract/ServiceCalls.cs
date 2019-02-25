using System.Collections.ObjectModel;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class ServiceCalls : KeyedCollection<int, ServiceCall>
    {
        protected override int GetKeyForItem(ServiceCall Item) => Item.Id;
    }
}
