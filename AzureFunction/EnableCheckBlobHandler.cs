namespace Lights.Function
{
    using Azure.Storage.Blobs;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class EnableCheckBlobHandler
    {
        private BlobContainerClient container { get; } = new BlobContainerClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "lights");
        private BlobClient blob 
        { 
            get { return container.GetBlobClient("enabled"); }
        }

        public async Task TurnOnOrOff(bool on)
        {
            await this.container.CreateIfNotExistsAsync();
            if (on)
            {
                await this.blob.UploadAsync(new MemoryStream(), true);
            }
            else
            {
                await this.blob.DeleteIfExistsAsync();
            }
        }

        public async Task<bool> IsOn()
        {
            return await this.blob.ExistsAsync();
        }
    }
}
