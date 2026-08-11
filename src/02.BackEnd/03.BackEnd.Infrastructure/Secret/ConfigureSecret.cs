using Pertamina.Services.Secret.JsonFile;
using Pertamina.Services.Secret.PertaminaVault;

namespace EBVL.BackEnd.Infrastructure.Secret;

public static class ConfigureSecret
{
    public static async Task<Dictionary<string, string>> GetSecretsAsync(this WebApplicationBuilder builder)
    {
        var secretProvider = builder.Configuration[$"{nameof(Secret)}:Provider"]
            ?? throw new ConfigurationBindingFailedException($"{nameof(Secret)}:Provider", typeof(string));

        if (secretProvider is SecretProvider.JsonFile)
        {
            var sectionKey = $"{nameof(Secret)}:{nameof(SecretProvider.JsonFile)}:FilePath";
            var filePath = builder.Configuration[sectionKey];

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw ExceptionFor.ConfigurationBindingFailed(sectionKey, typeof(string));
            }

            return await new JsonFileSecretService(filePath).GetSecretsAsync();
        }

        if (secretProvider is SecretProvider.PertaminaVault)
        {
            var pertaminaVaultSecretOptions = builder.Configuration.GetRequiredSection(PertaminaVaultSecretOptions.SectionKey).Get<PertaminaVaultSecretOptions>()
                ?? throw new ConfigurationBindingFailedException(PertaminaVaultSecretOptions.SectionKey, typeof(PertaminaVaultSecretOptions));

            return await new PertaminaVaultSecretService(pertaminaVaultSecretOptions).GetSecretsAsync();
        }

        throw new UnsupportedServiceProviderException(nameof(Secret), secretProvider);
    }
}
