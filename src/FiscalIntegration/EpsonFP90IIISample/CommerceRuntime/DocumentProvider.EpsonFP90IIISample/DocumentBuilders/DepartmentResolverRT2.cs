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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

        /// <summary>
        /// RT2 version of resolver for creating department element.
        /// </summary>
        internal class DepartmentResolverRT2 : IDepartmentResolver
        {
            /// <summary>
            /// Gets department mapping.
            /// </summary>
            public Dictionary<ExtendedVATRate, string> DepartmentsMapping { get; }

            /// <summary>
            /// Gets vat exempt nature for gift card.
            /// </summary>
            public string VatExemptNatureForGiftCard { get; }

            /// <summary>
            /// Constructor for DepartmentResolverRT2 class.
            /// </summary>
            /// <param name="functionalityProfile">Fiscal integration functionality profile.</param>
            public DepartmentResolverRT2(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                DepartmentsMapping = ConfigurationController.ParseSupportedDepartmentNumbersMappings(functionalityProfile);
                VatExemptNatureForGiftCard = ConfigurationController.ParseVATExemptNatureForGiftCard(functionalityProfile);
            }

            /// <summary>
            /// Gets the department number for fiscal printer.
            /// </summary>
            /// <param name="context">The current request context.</param>
            /// <param name="taxRatePercent">The tax rate.</param>
            /// <param name="receiptId">The receipt id.</param>
            /// <param name="productId">The product id.</param>
            /// <param name="isGiftCardLine">Indicates whether this instance is a gift card line.</param>
            /// <returns>Department number.</returns>
            public async Task<string> GetDepartmentNumberAsync(RequestContext context, decimal taxRatePercent,
                string receiptId, long productId, bool isGiftCardLine)
            {
                GetProductsDataRequest productDataRequest = new GetProductsDataRequest(new long[] {productId}, QueryResultSettings.SingleRecord);
                var response = await context.ExecuteAsync<EntityDataServiceResponse<SimpleProduct>>(productDataRequest).ConfigureAwait(false);

                string itemType = response.PagedEntityCollection.Results[0].ItemType == ReleasedProductType.Service
                    ? DocumentAttributeConstants.ServiceProductTypeValue
                    : DocumentAttributeConstants.ItemProductTypeValue;
                decimal taxRatePercentSalesLine = Math.Round(taxRatePercent, 2);
                var vatExemptNatureForGiftCard = isGiftCardLine ? VatExemptNatureForGiftCard : string.Empty;

                var key = new ExtendedVATRate(SalesOrderHelper.ConvertSalesTaxRateToInt(taxRatePercentSalesLine), vatExemptNatureForGiftCard, itemType);
                if (DepartmentsMapping.TryGetValue(key, out var department) == false)
                {
                    throw new ArgumentException(
                        $"The transaction {receiptId} couldn't be registered on a fiscal device or service due to the missing VAT data mapping (VAT rate percentage is {taxRatePercentSalesLine}). Please contact your system administrator.");
                }

                return department;
            }
        }
    }
}
