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
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;

        /// <summary>
        /// The request to populate EFR localization information to a payment.
        /// </summary>
        public sealed class PopulateEfrLocalizationInfoToPaymentRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PopulateEfrLocalizationInfoToPaymentRequest"/> class.
            /// </summary>
            /// <param name="receiptPayment">The receipt payment.</param>
            public PopulateEfrLocalizationInfoToPaymentRequest(ReceiptPayment receiptPayment)
            {
                this.ReceiptPayment = receiptPayment;
            }

            /// <summary>
            /// Gets the receipt payment.
            /// </summary>
            public ReceiptPayment ReceiptPayment { get; private set; }
        }
    }
}
