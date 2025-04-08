using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client.Data.Implementation;
using Client.Data.API; // Potrzebne dla IDataContext
using System;
using ClientServer.Shared.Data.API;

namespace Client.Data.Tests
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