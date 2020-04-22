using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.Cache;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using MediatR;

namespace Celo.RandomUserApi.Infrastructure.KeyVault
{
    public class GetKeyVaultSecret : IRequest<string>, ICacheable
    {
        public GetKeyVaultSecret(string secretName)
        {
            secretName.ThrowIfNullOrEmpty(nameof(secretName));
            SecretName = secretName;
            CacheSetting = new MemCacheSetting($"KeyVault-{SecretName}", TimeSpan.FromDays(1));
        }

        public string SecretName { get; }

        public ICacheSetting CacheSetting { get; }
    }

    public class GetKeyVaultSecretHandler : IRequestHandler<GetKeyVaultSecret, string>
    {
        // This is a stub implementation. We would use the AzureKeyVault SDK here for a production app.
        public Task<string> Handle(GetKeyVaultSecret request, CancellationToken cancellationToken)
        {
            if (KeyVaultSecrets.TryGetValue(request.SecretName, out var value))
                return Task.FromResult(value);
            throw new KeyNotFoundException($"{request.SecretName} cannot be found.");
        }

        private static readonly Dictionary<string,string> KeyVaultSecrets = new Dictionary<string, string>
        {
            {"CosmosDBKey","specify-key-here"}
        };

    }
}