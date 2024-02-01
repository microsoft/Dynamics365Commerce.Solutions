/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.Runtime.Extensions.AttributePricingServicesSample
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Data;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// This handler is used to collect pricing properties passed in sales transaction, 
        /// so these properties can be used for matching document.
        /// </summary>
        public class GetPricingPropertiesDataRequestHandler : SingleAsyncRequestHandler<GetPricingPropertiesDataRequest>
        {
            /// Executes the workflow to merge the pricing properties of sales transaction.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            protected override async Task<Response> Process(GetPricingPropertiesDataRequest request)
            {
                ThrowIf.Null(request, nameof(request));

                using (var databaseContext = new DatabaseContext(request.RequestContext))
                {
                    // Execute original functionality to get pricing properties.
                    await this.ExecuteNextAsync<NullResponse>(request).ConfigureAwait(false);

                    var getPricingAttributeLinkGroupRequest = new GetPricingAttributeLinkGroupDataRequest();

                    var response = await request.RequestContext.ExecuteAsync<GetPricingAttributeLinkGroupDataResponse>(getPricingAttributeLinkGroupRequest).ConfigureAwait(false);
                    var attributeLinkGroups = response.AttributeLinkGroups;

                    // Execute additional functionality to read sales transaction properties.
                    if (request.AttributeSource.SourceEntity is SalesTransaction salesTransaction)
                    {
                        const string PricingAttributeKeyToOverwrite = "AnySalesOrderAttribute";

                        // Arrange sales order transactions.
                        // This is what customer should do in contrtoller customization.
                        salesTransaction.SetProperty(PricingAttributeKeyToOverwrite, "AttributeValue");

                        this.CollectPricingPropertiesForSalesTransactionOrSalesLine(attributeLinkGroups, PricingAttributeSourceLevel.Header, PricingAttributeKeyToOverwrite, salesTransaction.GetProperty(PricingAttributeKeyToOverwrite), request);
                    }
                    else if (request.AttributeSource.SourceEntity is SalesLine salesLine)
                    {
                        const string PricingAttributeKeyToOverwrite = "AnySalesLineAttribute";

                        // Arrange sales line.
                        // This is what customer should do in contrtoller customization.
                        salesLine.SetProperty(PricingAttributeKeyToOverwrite, "AttributeValue");

                        this.CollectPricingPropertiesForSalesTransactionOrSalesLine(attributeLinkGroups, PricingAttributeSourceLevel.Line, PricingAttributeKeyToOverwrite, salesLine.GetProperty(PricingAttributeKeyToOverwrite), request);
                    }
                    else if (request.AttributeSource.SourceEntity is Customer)
                    {
                        const string CustomizedTableName = "CustomizedTableName";
                        const string CustomizedFieldName = "CustomizedFieldName";

                        await this.CollectPricingPropertiesForCustomerOrItemAsync(attributeLinkGroups, PricingAttributeSourceLevel.Header, CustomizedTableName, CustomizedFieldName, request, databaseContext).ConfigureAwait(false);
                    }
                    else if (request.AttributeSource.SourceEntity is Item)
                    {
                        const string CustomizedTableName = "CustomizedTableName";
                        const string CustomizedFieldName = "CustomizedFieldName";

                        await this.CollectPricingPropertiesForCustomerOrItemAsync(attributeLinkGroups, PricingAttributeSourceLevel.Line, CustomizedTableName, CustomizedFieldName, request, databaseContext).ConfigureAwait(false);
                    }

                    return NullResponse.Instance;
                }
            }

            private void CollectPricingPropertiesForSalesTransactionOrSalesLine(
                IEnumerable<PricingAttributeLinkGroup> attributeLinkGroups,
                PricingAttributeSourceLevel pricingAttributeSourceLevel,
                string pricingAtributeKey,
                object pricingAttributeValue,
                GetPricingPropertiesDataRequest request)
            {
                foreach (var attributeLinkGroup in attributeLinkGroups.Where(group => group.SourceLevel == pricingAttributeSourceLevel))
                {
                    foreach (var priceAttributeLink in attributeLinkGroup.PricingAttributeLinks.Where(link => link.AttributeName.Equals(pricingAtributeKey, StringComparison.OrdinalIgnoreCase)))
                    {
                        request.AttributeSource.PricingProperties[priceAttributeLink] = pricingAttributeValue;
                    }
                }
            }

            private async Task CollectPricingPropertiesForCustomerOrItemAsync(
                IEnumerable<PricingAttributeLinkGroup> attributeLinkGroups,
                PricingAttributeSourceLevel pricingAttributeSourceLevel,
                string customizedTableName,
                string customizedFieldName,
                GetPricingPropertiesDataRequest request,
                DatabaseContext databaseContext)
            {
                HashSet<PricingAttributeLink> pricingAttributes =
                    attributeLinkGroups
                        .Where(group => group.SourceLevel == pricingAttributeSourceLevel)
                        .Aggregate(
                            new HashSet<PricingAttributeLink>(),
                            (acc, cur) =>
                            {
                                acc.UnionWith(cur.PricingAttributeLinks.Where(link => link.FieldName == customizedFieldName));
                                return acc;
                            });

                if (pricingAttributes.Any())
                {
                    SqlPagedQuery query = null;

                    if (request.AttributeSource.SourceEntity is Customer customer)
                    {
                        query = new SqlPagedQuery(QueryResultSettings.AllRecords)
                        {
                            Select = new ColumnSet(pricingAttributes.Select(attr => attr.FieldName).ToArray()),
                            From = customizedTableName,
                            DatabaseSchema = "EXT",
                            Where = "ACCOUNTNUM = @accountNumber and DATAAREAID = @dataAreaId",
                            Parameters =
                            {
                                ["@accountNumber"] = customer.AccountNumber,
                                ["@dataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId,
                            },
                        };
                    }
                    else if (request.AttributeSource.SourceEntity is Item item)
                    {
                        query = new SqlPagedQuery(QueryResultSettings.AllRecords)
                        {
                            Select = new ColumnSet(pricingAttributes.Select(attr => attr.FieldName).ToArray()),
                            From = customizedTableName,
                            Where = "ITEMID = @itemId and DATAAREAID = @dataAreaId",
                            Parameters =
                            {
                                ["@itemId"] = item.ItemId,
                                ["@dataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId,
                            },
                        };
                    }

                    var pricingProperties = new SortedDictionary<IPricingAttribute, object>(PricingComparer.Instance);

                    await databaseContext
                        .ExecuteQueryWithRetryAsync(
                            query,
                            (result) =>
                            {
                                foreach (var pricingAttribute in pricingAttributes)
                                {
                                    pricingProperties[pricingAttribute] = result.GetValue<object>(pricingAttribute.FieldName);
                                }

                                return Task.CompletedTask;
                            })
                        .ConfigureAwait(false);

                    request.AttributeSource.Merge(pricingProperties);
                }
            }
        }
    }
}