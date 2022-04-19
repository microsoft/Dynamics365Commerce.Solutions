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
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.Documents
    {
        using System;
        using System.Xml.Serialization;

        /// <summary>
        /// The request document to register the start of a transaction.
        /// </summary>
        [Serializable]
        [XmlRoot(ElementName = RootElementName)]
        public class BeginSaleRegistrationRequest : IFiscalIntegrationDocument
        {
            private const string RootElementName = "TraS";
            private const string ReceiptElementName = "ESR";

            /// <summary>
            /// Gets or sets the receipt.
            /// </summary>
            [XmlElement(ElementName = ReceiptElementName)]
            public Receipt Receipt { get; set; }
        }
    }
}
