using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Soat.Eleven.Kutcut.Users.Api.Configurations;

public static class KeyVaultConfiguration
{
    public static WebApplicationBuilder ConfigureKeyVault(this WebApplicationBuilder builder)
    {
        var keyVaultName = builder.Configuration["KeyVault:Name"];

        var isDevelopmentEnvironment = builder.Environment.IsDevelopment();
        TokenCredential credential;

        if (isDevelopmentEnvironment)
        {
            Console.WriteLine("🔧 Ambiente de desenvolvimento detectado - usando configuração local");

            credential = new ChainedTokenCredential(
                new AzureCliCredential(),
                new EnvironmentCredential(),
                new ManagedIdentityCredential()
            );
        }
        else
        {
            Console.WriteLine("🔧 Ambiente de desenvolvimento detectado - usando configuração de Produção");

            credential = new DefaultAzureCredential();
        }

        if (!string.IsNullOrEmpty(keyVaultName))
        {
            SetKeyVault(builder, keyVaultName, credential);
            Console.WriteLine("🔐 Configuração do Key Vault para SecretKey carregada com sucesso.");
        }

        return builder;
    }

    private static void SetKeyVault(WebApplicationBuilder builder, string keyVaultName, TokenCredential credential)
    {
        try
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                credential,
                new KeyVaultSecretManager());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO : ao configurar o Key Vault: {ex.Message}");
            throw;
        }
    }
}

public class KeyVaultSecretManager : Azure.Extensions.AspNetCore.Configuration.Secrets.KeyVaultSecretManager
{
    public override string GetKey(KeyVaultSecret secret)
    {
        // Converte nomes do Key Vault para formato de configuração
        // Exemplo: ConnectionString--Database vira ConnectionString:Database
        return secret.Name.Replace("--", ":");
    }
}
