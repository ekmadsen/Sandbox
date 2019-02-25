using System.Collections.ObjectModel;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public class Technicians : KeyedCollection<int, Technician>
    {
        protected override int GetKeyForItem(Technician Item) => Item.Id;
    }
}
