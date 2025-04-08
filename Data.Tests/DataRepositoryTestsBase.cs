using Server.Data.API;
using ClientServer.Shared.Data.API;

namespace Server.Data.Tests
{
    [TestClass]
    public abstract class DataRepositoryTestBase
    {
        protected IDataContext _mockContext = null!;
        protected IDataRepository _repository = null!;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _mockContext = new MockDataContext();
            _repository = DataRepositoryFactory.CreateDataRepository(_mockContext);
        }

    }
}