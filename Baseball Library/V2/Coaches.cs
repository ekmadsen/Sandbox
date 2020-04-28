using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class Coaches : Collection<ICoach>, ICoaches
    {
        private readonly List<CoachRecord> _records;


        public Coaches(List<CoachRecord> Records = null)
        {
            _records = Records ?? new List<CoachRecord>();
            if (Records != null)
            {
                // Add to list of domain classes without triggering InsertItem (which would add duplicate record).
                foreach (CoachRecord record in Records) Items.Add(new Coach(record));
            }
        }


        protected override void InsertItem(int Index, ICoach Item)
        {
            base.InsertItem(Index, Item);
            if (Item is ICoachRecordPattern pattern) _records.Insert(Index, pattern.Record);
        }


        protected override void SetItem(int Index, ICoach Item)
        {
            base.SetItem(Index, Item);
            if (Item is ICoachRecordPattern pattern) _records[Index] = pattern.Record;
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
