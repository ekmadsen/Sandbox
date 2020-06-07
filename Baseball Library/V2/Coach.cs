// ReSharper disable UnusedMemberInSuper.Global
namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class Coach : TeamMemberBase, ICoach, ICoachRecordPattern
    {
        private ICoach _manager;
        private IPlayers _players;


        public CoachRecord Record { get; set; }


        public string Specialty
        {
            get => Record.Specialty;
            set => Record.Specialty = value;
        }


        public ICoach Manager
        {
            get => _manager;
            set
            {
                _manager = value;
                if (value is ICoachRecordPattern pattern) Record.Manager = pattern.Record;
            }
        }


        public IPlayers Players
        {
            get => _players;
            set
            {
                _players = value;
                Record.Players.Clear();
                foreach (IPlayer player in value) if (player is IPlayerRecordPattern pattern) Record.Players.Add(pattern.Record);
            }
        }


        public Coach() : this(new CoachRecord())
        {
        }


        public Coach(CoachRecord Record) : base(Record)
        {
            this.Record = Record;
            _players = new Players(this.Record.Players);
        }
    }


    internal interface ICoachRecordPattern
    {
        CoachRecord Record { get; set; }
    }
}
