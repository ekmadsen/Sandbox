using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class Players : Collection<IPlayer>, IPlayers
    {
        private readonly List<PlayerRecord> _records;


        public Players(List<PlayerRecord> Records = null)
        {
            _records = Records ?? new List<PlayerRecord>();
            if (Records != null)
            {
                // Add to list of domain classes without triggering InsertItem (which would add duplicate record).
                foreach (PlayerRecord record in Records) Items.Add(new Player(record));
            }
        }


        protected override void InsertItem(int Index, IPlayer Item)
        {
            base.InsertItem(Index, Item);
            if (Item is IPlayerRecordPattern pattern) _records.Insert(Index, pattern.Record);
        }


        protected override void SetItem(int Index, IPlayer Item)
        {
            base.SetItem(Index, Item);
            if (Item is IPlayerRecordPattern pattern) _records[Index] = pattern.Record;
        }


        protected override void RemoveItem(int Index)
        {
            base.RemoveItem(Index);
            _records.RemoveAt(Index);
        }


        protected override void ClearItems()
        {
            base.ClearItems();
            _records.Clear();
        }
    }
}
