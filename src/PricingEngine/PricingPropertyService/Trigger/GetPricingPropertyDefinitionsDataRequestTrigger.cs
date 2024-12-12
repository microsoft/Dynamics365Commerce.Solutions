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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Contoso.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// The GetPricingPropertyDefinitionsDataResponse trigger.
    /// </summary>
    public class GetPricingPropertyDefinitionsDataRequestTrigger : IRequestTriggerAsync
    {
        private const string ContosoPricingPropertyDefinitionsView = "CONTOSOPRICINGPROPERTYDEFINITIONSVIEW";

        private const string PricingPropertyDefinitionStatusColumn = "STATUS";
        private const string DataAreaIdColumn = "DATAAREAID";
        private const string PricingPropertyDefinitionStatusVariable = "@status";
        private const string DataAreaIdVariable = "@dataAreaId";

        /// <inheritdoc />
        public IEnumerable<Type> SupportedRequestTypes
        {
            get { return new[] { typeof(GetPricingPropertyDefinitionsDataRequest) }; }
        }


        /// <inheritdoc />
        public Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task OnExecuted(Request request, Response response)
        {
            var query = new SqlPagedQuery(request.QueryResultSettings)
            {
                From = ContosoPricingPropertyDefinitionsView,
                Where = $"{PricingPropertyDefinitionStatusColumn} = {PricingPropertyDefinitionStatusVariable} AND {DataAreaIdColumn} = {DataAreaIdVariable}",
            };

            query.Parameters[PricingPropertyDefinitionStatusVariable] = 0; // Published.
            query.Parameters[DataAreaIdVariable] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;

            using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
            {
                PagedResult<ContosoPricingPropertyDefinition> result = await databaseContext.ReadEntityAsync<ContosoPricingPropertyDefinition>(query).ConfigureAwait(false);

                GetPricingPropertyDefinitionsDataResponse origResponse = (GetPricingPropertyDefinitionsDataResponse)response;

                foreach (PricingPropertyDefinition pricingPropertyDefinition in origResponse.Definitions)
                {
                    pricingPropertyDefinition.ExtensionProperties.Add(new CommerceProperty(
                        "ContosoPropertyType",
                        (int)result.Single(definition => definition.RecordId == pricingPropertyDefinition.RecordId)
                            .ContosoPropertyType));
                }
            }
        }
    }
}