using System.Collections.ObjectModel;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class Customers : KeyedCollection<int, Customer>
    {
        protected override int GetKeyForItem(Customer Item) => Item.Id;
    }
}
