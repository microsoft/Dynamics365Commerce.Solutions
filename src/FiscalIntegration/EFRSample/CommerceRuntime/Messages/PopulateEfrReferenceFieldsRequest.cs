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
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The request to populate EFR reference fields.
        /// </summary>
        public sealed class PopulateEfrReferenceFieldsRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PopulateEfrReferenceFieldsRequest"/> class.
            /// </summary>
            /// <param name="receiptPosition">The receipt position.</param>
            /// <param name="salesLine">The sales line.</param>
            public PopulateEfrReferenceFieldsRequest(ReceiptPosition receiptPosition, SalesLine salesLine)
            {
                this.ReceiptPosition = receiptPosition;
                this.SalesLine = salesLine;
            }

            /// <summary>
            /// Gets the sales line.
            /// </summary>
            public SalesLine SalesLine { get; private set; }

            /// <summary>
            /// Gets the receipt position.
            /// </summary>
            public ReceiptPosition ReceiptPosition { get; private set; }
        }
    }
}
