/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample.Database
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Common;
    using Contoso.RetailServer.Ecommerce.AbandonedCartSample.Options;

    public class RunStatusContainer : IRunStatusContainer
    {

        private CosmosClient cosmosClient;

        private Database database;

        private Container container;

        private readonly AzureCosmosOptions azureCosmosOptions;

        private readonly ILogger<RunStatusContainer> logger;

        public RunStatusContainer(IOptions<AzureCosmosOptions> azureCosmosOptions,
                                  ILogger<RunStatusContainer> logger)
        {
            azureCosmosOptions.VerifyIsNotNull(nameof(azureCosmosOptions));
            ValidationHelpers.TryValidate(azureCosmosOptions.Value);
            logger.VerifyIsNotNull(nameof(logger));

            this.logger = logger;
            this.azureCosmosOptions = azureCosmosOptions.Value;
            this.cosmosClient = new CosmosClient(azureCosmosOptions.Value.EndPointUri, azureCosmosOptions.Value.PrimaryKey);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateContainerIfNotExistsAsync().Wait();
        }

        public async Task UpsertItemAsync(RunStatus item)
        {
            await this.container.UpsertItemAsync<RunStatus>(item, new PartitionKey(item.Oun)).ConfigureAwait(false);
        }

        public async Task DeleteItemAsync(RunStatus item)
        {
            await this.container.DeleteItemAsync<RunStatus>(item.Id, new PartitionKey(item.Oun)).ConfigureAwait(false);
        }

        public async Task<RunStatus> GetItemAsync(string id, string partitionKey)
        {
            try
            {
                return await this.container.ReadItemAsync<RunStatus>(id, new PartitionKey(partitionKey)).ConfigureAwait(false);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            this.database = await cosmosClient.CreateDatabaseIfNotExistsAsync(this.azureCosmosOptions.DatabaseId).ConfigureAwait(false);
            this.logger.LogInformation("Created Database: {0}\n", this.database.Id);
        }

        private async Task CreateContainerIfNotExistsAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(this.azureCosmosOptions.ContainerId, "/oun").ConfigureAwait(false);
            this.logger.LogInformation("Created Container: {0}\n", this.azureCosmosOptions.ContainerId);
        }
    }

}
