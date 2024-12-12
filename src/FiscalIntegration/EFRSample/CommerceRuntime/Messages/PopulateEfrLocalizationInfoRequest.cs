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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Messages
    {
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        public sealed class PopulateEfrLocalizationInfoRequest: Request
        {
            /// <summary>
            /// Initializes a new instance of <see cref="PopulateEfrLocalizationInfoRequest"/>.
            /// </summary>
            public PopulateEfrLocalizationInfoRequest(DataModelEFR.Documents.Receipt receipt, SalesOrder salesOrder)
            {
                this.Receipt = receipt;
                this.SalesOrder = salesOrder;
            }

            public DataModelEFR.Documents.Receipt Receipt { get; private set; }

            public SalesOrder SalesOrder { get; private set; }
        }
    }
    
}
