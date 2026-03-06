using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.UnitTests.Common
{
    class MockKeyMaterialService : IKeyMaterialService
    {
        public List<SigningCredentials> SigningCredentials = new();
        public List<SecurityKeyInfo> ValidationKeys = new();

        public Task<IReadOnlyCollection<SigningCredentials>> GetAllSigningCredentialsAsync()
        {
            return Task.FromResult((IReadOnlyCollection<SigningCredentials>)SigningCredentials);
        }

        public Task<SigningCredentials?> GetSigningCredentialsAsync(ICollection<string>? allowedAlgorithms = null)
        {
            return Task.FromResult(SigningCredentials.FirstOrDefault());
        }

        public Task<IReadOnlyCollection<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            return Task.FromResult((IReadOnlyCollection<SecurityKeyInfo>)ValidationKeys);
        }
    }
}
