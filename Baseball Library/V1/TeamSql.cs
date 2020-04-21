using System.Collections.Generic;
using System.Data;
using Moq;


namespace ErikTheCoder.Sandbox.Baseball.Library.V1
{
    internal class TeamSql : TeamBase
    {
        protected override void CreateInternal() => throw new System.NotImplementedException();
        protected override void UpdateInternal() => throw new System.NotImplementedException();
        protected override void DeleteInternal() => throw new System.NotImplementedException();


        protected override void ReadInternal()
        {
            // Mock reading from a database.



        }


        private IDataReader MockIDataReader(List<TestData> ojectsToEmulate)
        {
            var moq = new Mock<IDataReader>();

            // This var stores current position in 'ojectsToEmulate' list
            int count = -1;

            moq.Setup(x => x.Read())
                // Return 'True' while list still has an item
                .Returns(() => count < ojectsToEmulate.Count - 1)
                // Go to next position
                .Callback(() => count++);

            moq.Setup(x => x["Char"])
                // Again, use lazy initialization via lambda expression
                .Returns(() => ojectsToEmulate[count].ValidChar);

            return moq.Object;
        }
    }
}
