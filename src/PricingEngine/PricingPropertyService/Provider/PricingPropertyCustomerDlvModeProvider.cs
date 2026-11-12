/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.Services.Pricing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Contoso.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;

    /// <summary>
    /// The customer delivery mode provider.
    /// </summary>
    public class PricingPropertyCustomerDlvModeProvider : SingleAsyncRequestHandler<ResolvePricingPropertyValuesServiceRequest>, IPricingPropertyAware
    {
        /// <inheritdoc />
        public bool IsPropertySupported(PricingPropertyDefinition definition)
        {
            return definition.PropertyLevel == PricingPropertyLevel.Header
                   && definition.PropertyType == PricingPropertyType.Extension
                   && definition.ExtensionProperties.SingleOrDefault(p => p.Key == "ContosoPropertyType")?.Value?.IntegerValue == (int)ContosoPropertyType.CustDlvMode;
        }

        /// <inheritdoc />
        protected override async Task<Response> Process(ResolvePricingPropertyValuesServiceRequest request)
        {
            SalesTransaction salesTransaction = request.Transaction;

            PricingPropertyDefinition customerDlvModeProperty =
                request.PricingProperties.Where(this.IsPropertySupported).Single();

            ResolvePricingPropertyValuesServiceResponse response = new ResolvePricingPropertyValuesServiceResponse();
            if (salesTransaction.CustomerId.IsNullOrEmpty())
            {
                return response;
            }

            var query = new SqlPagedQuery(request.QueryResultSettings)
            {
                DatabaseSchema = "ext",
                From = "CUSTDLVMODEVIEW",
                Where = $"ACCOUNTNUM = @tvp_AccountNum AND DATAAREAID = tvp_DataAreaId",
            };

            query.Parameters["@tvp_AccountNum"] = salesTransaction.CustomerId; // Published.
            query.Parameters["@tvp_DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;

            using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
            {
                PagedResult<ContosoCustomerDlvMode> result = await databaseContext.ReadEntityAsync<ContosoCustomerDlvMode>(query).ConfigureAwait(false);
                response.HeaderPropertyValues.Add(customerDlvModeProperty, new[] { CommercePropertyValue.FromObject(result.Single().DlvMode) });
            }

            return response;
        }
    }
}