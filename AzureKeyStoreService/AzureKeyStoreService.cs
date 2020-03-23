using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureCoreService
{
    public static class AzureKeyStoreService
    {
        public static void SetupAzureKeyVaultConfiguration(IConfigurationBuilder config, bool required = false, bool debug = false)
        {
            var root = config.Build();
            var clientId = root["Azure:AzureADClientId"];
            var clientSecret = root["Azure:AzureADClientSecret"];
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                config.AddAzureKeyVault(root["Azure:KeyVaultUrl"], clientId, clientSecret);
            }
            else
            {
                var azureServiceTokenProvider =
                    new AzureServiceTokenProvider("RunAs=Developer; DeveloperTool=AzureCli");
                //var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient =
                    new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                config.AddAzureKeyVault($"{root["Azure:KeyVaultUrl"]}", keyVaultClient,
                    new DefaultKeyVaultSecretManager());
            }

            config.Build();
        }

        public static void ConfigureCookieAuthKeyStore(IServiceCollection services, IConfiguration configuration)
        {
            var clientId = configuration["Azure:AzureADClientId"];
            var clientSecret = configuration["Azure:AzureADClientSecret"];
            KeyVaultClient keyVaultClient = null;

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
                {
                    var authContext = new AuthenticationContext(authority);
                    var clientCred = new ClientCredential(configuration["Azure:AzureADClientId"],
                        configuration["Azure:AzureADClientSecret"]);
                    var result = await authContext.AcquireTokenAsync(resource, clientCred);

                    if (result == null)
                        throw new InvalidOperationException("Failed to obtain the JWT token");
                    return result.AccessToken;
                });
            }
            else
            {
                var azureServiceTokenProvider =
                    new AzureServiceTokenProvider("RunAs=Developer; DeveloperTool=AzureCli");
                keyVaultClient =
                    new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            }
                


            //var certificateSecret = keyVaultClient
            //    .GetSecretAsync(configuration["Azure:KeyVaultUrl"], configuration["Education-Portal-Staging-Auth-Cert"])
            //    .GetAwaiter()
            //    .GetResult();
            
            //var privateKeyBytes = Convert.FromBase64String(certificateSecret.Value);

            //config.AddEnvironmentVariables("test", certificateSecret.Value);

            //var authCertificate = new X509Certificate2(privateKeyBytes, (string)null);


            services.AddDataProtection()
                .SetApplicationName(configuration["ApplicationName"])
                //.PersistKeysToStackExchangeRedis(redis, $"{configuration["ApplicationName"]}-auth-cert")
                //.DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(configuration["SessionDataProtectionKeyPath"]))
                //.ProtectKeysWithCertificate(authCertificate)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365));
        }
    }
}
