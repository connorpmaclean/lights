namespace Lights.Core.Lifx
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Newtonsoft.Json;

    public static class LifxAccess
    {
        private static Lazy<Task<string>> token = new Lazy<Task<string>>(GetLifxApiKey);

        public static Task<string> GetTokenLazy()
        {
            return token.Value;
        } 

        private static async Task<string> GetLifxApiKey()
        {
            string keyVaultUrl = "https://cmlightskeyvault.vault.azure.net/";
            string connectionString = "RunAs=Developer; DeveloperTool=AzureCli";
            if (Environment.GetEnvironmentVariable("Azure") == "true")
            {
                connectionString = "RunAs=App;AppId=4624367f-cbd5-4bc8-a03b-df215c82084d";
            }

            var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);
            using (var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)))
            {
                var secret = await client.GetSecretAsync(keyVaultUrl, "LifxApiKey");
                return secret.Value;
            }
        }
    }
}