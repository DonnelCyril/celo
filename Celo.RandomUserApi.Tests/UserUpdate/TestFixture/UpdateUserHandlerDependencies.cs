using Celo.RandomUserApi.Infrastructure.Cdn;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using MediatR;

namespace Celo.RandomUserApi.UnitTests.UserUpdate.TestFixture
{
    public class UpdateUserHandlerDependencies
    {
        private readonly IMediator _mediator;
        private readonly IUserStoreFactory _userStoreFactory;
        private readonly ICdnCachePurge _cdnCachePurge;

        public UpdateUserHandlerDependencies(IMediator mediator, IUserStoreFactory userStoreFactory, ICdnCachePurge cdnCachePurge)
        {
            _mediator = mediator;
            _userStoreFactory = userStoreFactory;
            _cdnCachePurge = cdnCachePurge;
        }

        public void Deconstruct(out IMediator mediator, out IUserStoreFactory userStoreFactory, out ICdnCachePurge cdnCachePurge)
        {
            mediator = _mediator;
            userStoreFactory = _userStoreFactory;
            cdnCachePurge = _cdnCachePurge;
        }
    }
}