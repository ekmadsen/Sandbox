namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class Player : TeamMemberBase, IPlayer, IPlayerRecordPattern
    {
        public PlayerRecord Record { get; set; }


        public EPosition Position
        {
            get => Record.Position;
            set => Record.Position = value;
        }


        public int JerseyNumber
        {
            get => Record.JerseyNumber;
            set => Record.JerseyNumber = value;
        }


        public EHanded Bats
        {
            get => Record.Bats;
            set => Record.Bats = value;
        }


        public EHanded Throws
        {
            get => Record.Throws;
            set => Record.Throws = value;
        }


        public Player() : this(new PlayerRecord())
        {
        }


        public Player(PlayerRecord Record) : base(Record)
        {
            this.Record = Record;
        }
    }


    internal interface IPlayerRecordPattern
    {
        PlayerRecord Record { get; set; }
    }
}
