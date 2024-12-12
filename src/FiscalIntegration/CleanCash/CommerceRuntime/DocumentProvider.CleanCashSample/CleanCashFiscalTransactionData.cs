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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Clean cash fiscal transaction data structure class.
        /// </summary>
        [DataContract]
        public class CleanCashFiscalTransactionData
        {
            /// <summary>
            /// Gets or sets the organization number.
            /// </summary>
            [DataMember]
            public string StoreTaxRegNumber { get; set; }

            /// <summary>
            /// Gets or sets the terminal ID.
            /// </summary>
            [DataMember]
            public string TerminalId { get; set; }

            /// <summary>
            /// Gets or sets the transaction date.
            /// </summary>
            [DataMember]
            public string TransactionDate { get; set; }

            /// <summary>
            /// Gets or sets the receipt ID.
            /// </summary>
            [DataMember]
            public string ReceiptId { get; set; }

            /// <summary>
            /// Gets or sets the transaction type.
            /// </summary>
            [DataMember]
            public bool IsCopy { get; set; }

            /// <summary>
            /// Gets or sets the total amount.
            /// </summary>
            [DataMember]
            public string TotalAmount { get; set; }

            /// <summary>
            /// Gets or sets the negative amount.
            /// </summary>
            [DataMember]
            public string NegativeAmount { get; set; }

            /// <summary>
            /// Gets or sets the VAT1 field.
            /// </summary>
            [DataMember]
            public string VAT1 { get; set; }

            /// <summary>
            /// Gets or sets the VAT2 field.
            /// </summary>
            [DataMember]
            public string VAT2 { get; set; }

            /// <summary>
            /// Gets or sets the VAT3 field.
            /// </summary>
            [DataMember]
            public string VAT3 { get; set; }

            /// <summary>
            /// Gets or sets the VAT4 field.
            /// </summary>
            [DataMember]
            public string VAT4 { get; set; }
        }

    }
}
