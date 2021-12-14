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
    namespace CommerceRuntime.DocumentProvider.SequentialSignNorway
    {
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Document builder.
        /// </summary>
        internal class DocumentBuilder
        {
            /// <summary>
            /// Sequential signature key.
            /// </summary>
            internal const string SequenceKey = "LAST_SEQUENTIAL_SIGNATURE_F9FEE713-C7A4-4B16-8000-B80E5E099984";
            private const string FirstSignatureValue = "0";

            private readonly FiscalIntegrationSequentialSignatureData previousSequentialSignature;
            private readonly SalesOrder adjustedSalesOrder;

            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentBuilder" /> class.
            /// </summary>
            /// <param name="adjustedSalesOrder">The sales order to register.</param>
            /// <param name="documentContext">The document context.</param>
            internal DocumentBuilder(SalesOrder adjustedSalesOrder, FiscalIntegrationDocumentContext documentContext)
            {
                ThrowIf.Null(adjustedSalesOrder, nameof(adjustedSalesOrder));

                this.adjustedSalesOrder = adjustedSalesOrder;
                this.previousSequentialSignature = documentContext?.SignatureData?.Single(signature => signature.SequenceKey == SequenceKey);
            }

            /// <summary>
            /// Gets the sequential number of registrable event.
            /// </summary>
            internal long SequentialNumber => (this.previousSequentialSignature?.SequentialNumber ?? 0) + 1;

            /// <summary>
            /// Gets the last sequential signature response.
            /// </summary>
            internal string LastRegisteredResponse => this.previousSequentialSignature?.LastRegisterResponse ?? string.Empty;

            /// <summary>
            /// Gets a string representation of the data to register.
            /// </summary>
            /// <returns>A string representation of the data to register.</returns>
            internal Task<string> GetDataToRegister()
            {
                List<string> dataToRegisterFields = new List<string>();
                string lastSignatureFromResponse = string.IsNullOrEmpty(this.LastRegisteredResponse) ?
                    FirstSignatureValue : SequentialSignatureDataFormatter.GetLastSignatureFromResponse(this.LastRegisteredResponse);
                IEnumerable<SalesLine> salesLines = this.adjustedSalesOrder.ActiveSalesLines.Where(line => !line.IsInvoiceLine);

                dataToRegisterFields.Add(lastSignatureFromResponse);
                dataToRegisterFields.Add(SequentialSignatureDataFormatter.GetFormattedDate(this.adjustedSalesOrder.TransactionDateTime));
                dataToRegisterFields.Add(SequentialSignatureDataFormatter.GetFormattedTime(this.adjustedSalesOrder.TransactionDateTime));
                dataToRegisterFields.Add(SequentialSignatureDataFormatter.GetFormattedSequentialNumber(this.SequentialNumber));
                dataToRegisterFields.Add(SequentialSignatureDataFormatter.GetFormattedAmount(salesLines.Sum(line => line.TotalAmount)));
                dataToRegisterFields.Add(SequentialSignatureDataFormatter.GetFormattedAmount(salesLines.Sum(line => line.TotalAmount - line.TaxAmount)));

                return Task.FromResult(SequentialSignatureDataFormatter.GetFormattedDataToRegister(dataToRegisterFields));
            }
        }
    }
}
