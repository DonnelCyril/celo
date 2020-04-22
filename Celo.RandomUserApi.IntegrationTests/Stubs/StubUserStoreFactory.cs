using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.Infrastructure.CosmosDB.Model;

namespace Celo.RandomUserApi.IntegrationTests.Stubs
{
    public class StubUserStoreFactory : IUserStoreFactory
    {
        private readonly UserRecord _userRecord;

        public StubUserStoreFactory(UserRecord userRecord)
        {
            _userRecord = userRecord;
        }

        public Task<IUserStore> GetUserStore()
        {
            IUserStore userStore = new StubUserStore(_userRecord);
            return Task.FromResult(userStore);
        }
    }
}