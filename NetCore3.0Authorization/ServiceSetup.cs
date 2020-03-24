using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace NetCore3_0Authorization
{
    public static class ServiceSetup
    {
        public static void ConfigureCookieAuthKeyStore(IServiceCollection services, IConfiguration configuration)
        {
            var kv = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var authContext = new AuthenticationContext(authority);
                var clientCred = new ClientCredential(configuration["Azure:AzureADClientId"],
                    configuration["Azure:AzureADClientSecret"]);
                var result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                return result.AccessToken;
            });


            var certificateSecret = kv
                .GetSecretAsync(configuration["Azure:KeyVaultUrl"], configuration["Azure:EducationPortalAuthCertName"])
                .GetAwaiter()
                .GetResult();
            ;
            var privateKeyBytes = Convert.FromBase64String(certificateSecret.Value);

            //config.AddEnvironmentVariables("test", certificateSecret.Value);

            var authCertificate = new X509Certificate2(privateKeyBytes, (string) null);


            services.AddDataProtection()
                .SetApplicationName(configuration["ApplicationName"])
                //.PersistKeysToStackExchangeRedis(redis, $"{configuration["ApplicationName"]}-auth-cert")
                //.DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(configuration["SessionDataProtectionKeyPath"]))
                .ProtectKeysWithCertificate(authCertificate)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365));
        }
    }

    public class AuthClaimsToUserMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthClaimsToUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context?.User?.Claims != null && (context.User?.Claims).Any())
                // JK 9/5/2019 : Prevent the AD user? Why is this happening now?
                if (context.User.Identity.AuthenticationType == "Cookies")
                {
                    context.Items.Add("UserAuth",
                        GenericUserClaimsService.GetUserFromClaims(context.User.Claims.ToList()));
                    await _next.Invoke(context);
                    return;
                }

            //context.Response.StatusCode = 401; //UnAuthorized
            //await context.Response.WriteAsync("");
            //return;

            await _next.Invoke(context); // anon endpoint
        }
    }
}