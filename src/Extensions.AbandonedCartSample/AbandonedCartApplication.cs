/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using RetailServer.Ecommerce.AbandonedCartSample.Common;
    using RetailServer.Ecommerce.AbandonedCartSample.Database;
    using RetailServer.Ecommerce.AbandonedCartSample.Messaging;
    using RetailServer.Ecommerce.AbandonedCartSample.Options;
    using RetailServer.Ecommerce.AbandonedCartSample.Security;
    using Microsoft.Dynamics.Commerce.RetailProxy;
    using Microsoft.Dynamics.Commerce.RetailProxy.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class AbandonedCartApplication
    {
        private ManagerFactory factory;
        private ChannelConfiguration orgUnitConfiguration;

        private readonly ILogger<AbandonedCartApplication> logger;
        private readonly RetailServerClientOptions retailServerClientOptions;
        private readonly ITokenProvider tokenProvider;
        private readonly IRunStatusContainer runStatusContainer;
        private readonly IEmailProvider emailProvider;

        public AbandonedCartApplication(ILogger<AbandonedCartApplication> logger,
                                        ITokenProvider tokenProvider,
                                        IOptions<RetailServerClientOptions> retailServerClientOptions,
                                        IRunStatusContainer runStatusContainer,
                                        IEmailProvider emailProvider)
        {
            retailServerClientOptions.VerifyIsNotNull(nameof(retailServerClientOptions));
            ValidationHelpers.TryValidate(retailServerClientOptions.Value);

            runStatusContainer.VerifyIsNotNull(nameof(runStatusContainer));
            tokenProvider.VerifyIsNotNull(nameof(tokenProvider));
            logger.VerifyIsNotNull(nameof(logger));
            emailProvider.VerifyIsNotNull(nameof(emailProvider));

            this.logger = logger;
            this.retailServerClientOptions = retailServerClientOptions.Value;
            this.tokenProvider = tokenProvider;
            this.runStatusContainer = runStatusContainer;
            this.emailProvider = emailProvider;
        }

        public async Task Run(string[] args)
        {
            // Setup initial context and create factory
            this.factory = await this.CreateManagerFactory().ConfigureAwait(false);
            this.factory.VerifyIsNotNull(nameof(factory));

            this.orgUnitConfiguration = await this.GetOrgUnitConfiguration().ConfigureAwait(false);
            this.orgUnitConfiguration.VerifyIsNotNull(nameof(orgUnitConfiguration));

            // Get input criteria for cart search
            var recordId = string.Concat(this.retailServerClientOptions.OperatingUnitNumber, ":", this.orgUnitConfiguration.RecordId);

            var lastRunStatusFromDB = await this.runStatusContainer.GetItemAsync(recordId,
                                                                        this.retailServerClientOptions.OperatingUnitNumber).ConfigureAwait(false);

            var LastModifiedDateTimeFrom = lastRunStatusFromDB?.LastSuceessfulCompletionDateTime ??
                                                new DateTimeOffset(DateTime.UtcNow.AddMinutes(-this.retailServerClientOptions.IncludeAbandonedCartsModifiedSinceLastMinutes));

            var lastProcessedCartIds = lastRunStatusFromDB?.LastProcessedCartIds?.Split(',').ToList() ?? new List<string>();

            var LastModifiedDateTimeTo = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-this.retailServerClientOptions.ExcludeAbandonedCartsModifiedSinceLastMinutes));
            int batchSize = 100;
            int skipCount = 0;

            // Search abandoned carts to begin.
            var abandonedCarts = await this.SearchAbandonedCarts(LastModifiedDateTimeFrom, LastModifiedDateTimeTo, lastProcessedCartIds, batchSize, skipCount).ConfigureAwait(false);

            while (abandonedCarts.Any())
            {
                // Get customer info for abandoned carts.
                var customers = await GetCustomerDetails(abandonedCarts.Select(cart => cart.CustomerId).ToList()).ConfigureAwait(false);
                ValidationHelpers.VerifyIsNotNullOrEmpty(customers, "customer data for abandoned carts");
                if (customers.Results.Count() != abandonedCarts.Count)
                {
                    throw new Exception("For abandoned carts, customer data must be available.");
                }

                // Find products ids from abandoned carts, remove duplicates.
                var productIds = new ObservableCollection<long>(abandonedCarts.SelectMany(cart => cart.CartLines
                                                                      .Where(cartLine => cartLine.ProductId != null)
                                                                      .GroupBy(cartLine => cartLine.ProductId.Value)
                                                                      .Select(groupedProductsIds => groupedProductsIds.Key)));

                var products = await this.GetProductDetails(productIds).ConfigureAwait(false);
                ValidationHelpers.VerifyIsNotNullOrEmpty(products, "products in abandoned carts");

                // Call email provider to send reminder emails.
                var resposnse = await this.emailProvider.SendEmails(abandonedCarts,
                                                                    customers,
                                                                    products,
                                                                    retailServerClientOptions.ReturnToCartUrl,
                                                                    this.orgUnitConfiguration.Currency).ConfigureAwait(false);
                if (resposnse != HttpStatusCode.OK)
                {
                    throw new Exception("Received non-success response from email provider while trying to send abandoned cart reminder emails.");
                }

                // Update variables for next iteration.
                // Get last processed cart ids from last modified timestamp to exclude them from next run 
                LastModifiedDateTimeFrom = (DateTimeOffset)abandonedCarts.Last().ModifiedDateTime;
                lastProcessedCartIds = abandonedCarts.Where(cart => cart.ModifiedDateTime == LastModifiedDateTimeFrom).Select(cart => cart.Id).ToList();
                var runStatus = new RunStatus
                {
                    Id = recordId,
                    LastSuceessfulCompletionDateTime = LastModifiedDateTimeFrom,
                    LastProcessedCartIds = string.Join(",", lastProcessedCartIds),
                    Oun = this.retailServerClientOptions.OperatingUnitNumber
                };
                await this.runStatusContainer.UpsertItemAsync(runStatus).ConfigureAwait(false);

                // Search abandoned carts for next iteration.
                abandonedCarts = await this.SearchAbandonedCarts(LastModifiedDateTimeFrom, LastModifiedDateTimeTo, lastProcessedCartIds, batchSize, skipCount).ConfigureAwait(false);
            }
        }

        private async Task<List<Cart>> SearchAbandonedCarts(DateTimeOffset lastModifiedDateTimeFrom,
                                                                   DateTimeOffset lastModifiedDateTimeTo,
                                                                   IEnumerable<string> lastProcessedCartIds,
                                                                   int batchSize,
                                                                   int skipCount)
        {
            ICartManager cartManager = this.factory.GetManager<ICartManager>();
            var criteria = new CartSearchCriteria
            {
                CartTypeValue = (int)CartType.Shopping,                
                LastModifiedDateTimeFrom = lastModifiedDateTimeFrom,
                LastModifiedDateTimeTo = lastModifiedDateTimeTo,
                IncludeAnonymous = false
            };

            var sortColumns = new ObservableCollection<SortColumn>();
            sortColumns.Add(new SortColumn() { ColumnName = "ModifiedDateTime", IsDescending = false });

            var settings = new QueryResultSettings
            {
                Paging = new PagingInfo
                {
                    Top = batchSize,
                    Skip = skipCount
                },
                Sorting = new SortingInfo()
                {
                    Columns = sortColumns
                }
            };

            var cartSearchResult = await cartManager.Search(criteria, settings).ConfigureAwait(false);
            if (cartSearchResult?.Results == null)
            {
                return new List<Cart>();
            }

            var carts = cartSearchResult?.Results;

            // Remove any cart ids that were processed in earlier batch and not been modified since.
            // Remmove any cart without customer id customer id
            // Remove any cart ids that do not have cart lines
            // If there are multiple carts for given customer, pick the latest modified one.
            // Since the carts are already sorted in ascending order, we just need to pick the last one.
            var abandonedCarts = (from cart in carts
                                  where cart.CustomerId != string.Empty &&
                                        cart.CartLines.Count > 0 &&
                                        cart.ModifiedDateTime != null &&
                                        (lastProcessedCartIds == null || cart.ModifiedDateTime > lastModifiedDateTimeFrom || !lastProcessedCartIds.Contains(cart.Id))
                                  group cart by cart.CustomerId into groupedCartsPerCustomer
                                  select groupedCartsPerCustomer.Last()).ToList();

            return abandonedCarts;
        }

        private async Task<PagedResult<Customer>> GetCustomerDetails(List<string> customerAccountNumbers)
        {
            var customerManager = this.factory.GetManager<ICustomerManager>();
            var settings = new QueryResultSettings
            {
                Paging = new PagingInfo
                {
                    Top = customerAccountNumbers.Count,
                    Skip = 0
                }
            };

            return await customerManager.GetByAccountNumbers(customerAccountNumbers, (int)SearchLocation.Local, settings).ConfigureAwait(false);
        }

        private async Task<PagedResult<Product>> GetProductDetails(ObservableCollection<long> productIds)
        {
            var productManager = this.factory.GetManager<IProductManager>();
            var settings = new QueryResultSettings
            {
                Paging = new PagingInfo
                {
                    Top = productIds.Count,
                    Skip = 0
                }
            };

            var productSearchCriteria = new ProductSearchCriteria()
            {
                SkipVariantExpansion = true,
                IncludeProductsFromDescendantCategories = false,
                DataLevelValue = (int)CommerceEntityDataLevel.Standard,
                Ids = productIds,
                IncludeAttributes = false,
                Context = new ProjectionDomain() { ChannelId = this.orgUnitConfiguration.RecordId }
            };

            return await productManager.Search(productSearchCriteria, settings).ConfigureAwait(false);
        }

        private async Task<ManagerFactory> CreateManagerFactory()
        {
            var CommerceApiEndpoint = new Uri(this.retailServerClientOptions.RetailServerUri);
            var retailApiAccessToken = await this.tokenProvider.GetTokenAsync(
                        retailServerClientOptions.AppIdKeyVaultSecretName,
                        retailServerClientOptions.AppSecretKeyVaultSecretName,
                        retailServerClientOptions.TenantId,
                        retailServerClientOptions.RetailServerAudienceId).ConfigureAwait(false);
            ValidationHelpers.VerifyIsNotNullOrWhiteSpace(retailApiAccessToken, nameof(retailApiAccessToken));

            ClientCredentialsToken clientCredentialsToken = new ClientCredentialsToken(retailApiAccessToken);
            RetailServerContext retailServerContext = RetailServerContext.Create(CommerceApiEndpoint, this.retailServerClientOptions.OperatingUnitNumber, clientCredentialsToken);
            return ManagerFactory.Create(retailServerContext);
        }

        private async Task<ChannelConfiguration> GetOrgUnitConfiguration()
        {
            var orgUnitManager = factory.GetManager<IOrgUnitManager>();
            return await orgUnitManager.GetOrgUnitConfiguration().ConfigureAwait(false);
        }
    }
}


