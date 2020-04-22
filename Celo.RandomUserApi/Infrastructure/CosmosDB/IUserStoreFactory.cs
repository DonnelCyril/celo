using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.KeyVault;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Celo.RandomUserApi.Infrastructure.CosmosDB
{
    public interface IUserStoreFactory
    {
        public Task<IUserStore> GetUserStore();
    }

    public class CosmosDBUserStoreFactory : IUserStoreFactory
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public CosmosDBUserStoreFactory(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        public async Task<IUserStore> GetUserStore()
        {

            var cosmosDBKey = await _mediator.Send(new GetKeyVaultSecret("CosmosDBKey"));
            var endpointUri = _environment.EnvironmentName switch
            {
                "Production" => "https://demo-store-prod.documents.azure.com:443/",
                _ => "https://demo-store.documents.azure.com:443/"
            };
            return new CosmosDBUserStore(endpointUri, cosmosDBKey);
        }
    }
}