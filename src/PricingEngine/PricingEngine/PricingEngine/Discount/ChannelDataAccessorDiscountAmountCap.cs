/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Contoso.CommerceRuntime.PricingEngine
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.Data.Types;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;

    /// <summary>
    /// Channel data accessor for amount cap discount.
    /// </summary>
    public class ChannelDataAccessorDiscountAmountCap : IDataAccessorDiscountAmountCap
    {
        /// <summary>
        /// The view needs to be created in channel DB following https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/channel-db-extensions.
        /// </summary>
        private const string DiscountAmountCapViewName = "CONTOSOAMOUNTCAPDISCOUNTVIEW";

        private readonly RequestContext requestContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataAccessorDiscountAmountCap" /> class.
        /// </summary>
        /// <param name="requestContext">Commerce runtime request context.</param>
        public ChannelDataAccessorDiscountAmountCap(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        /// <inheritdoc/>
        public async Task<object> GetDiscountAmountCapsByOfferIdsAsync(IEnumerable<string> offerIds)
        {
            using (SimpleProfiler profiler = new SimpleProfiler("GetDiscountAmountCapsByOfferIds", 1))
            {
                var query = new SqlPagedQuery(QueryResultSettings.AllRecords)
                {
                    DatabaseSchema = "ext",
                    From = DiscountAmountCapViewName,
                    Where = "DATAAREAID = @dataAreaId",
                };

                using (StringIdTableType type = new StringIdTableType(offerIds, "OFFERID"))
                using (var dbContext = new DatabaseContext(this.requestContext))
                {
                    query.Parameters["@TVP_OFFERIDTABLETYPE"] = type;
                    query.Parameters["@dataAreaId"] = this.requestContext.GetChannelConfiguration().InventLocationDataAreaId;

                    return await dbContext.ReadEntityAsync<DiscountAmountCap>(query).ConfigureAwait(false);
                }
            }
        }
    }
}
