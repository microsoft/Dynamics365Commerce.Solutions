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
        /// The sales transaction registration response.
        /// </summary>
        [Serializable]
        [XmlRoot(ElementName = RootElementName)]
        public class SalesTransactionRegistrationResponse
        {
            private const string RootElementName = "TraC";
            private const string SequenceNumberAttributeName = "SQ";
            private const string RegistrationResultElementName = "Result";
            private const string FiscalDataElementName = "Fis";
            private const string EfstaSimpleReceiptElementName = "ESR";

            /// <summary>
            /// Gets or sets the sequence number.
            /// </summary>
            [XmlAttribute(AttributeName = SequenceNumberAttributeName)]
            public int SequenceNumber { get; set; }

            /// <summary>
            /// Gets or sets the registration result.
            /// </summary>
            [XmlElement(ElementName = RegistrationResultElementName)]
            public RegistrationResult RegistrationResult { get; set; }

            /// <summary>
            /// Gets or sets the fiscal data.
            /// </summary>
            [XmlElement(ElementName = FiscalDataElementName)]
            public FiscalData FiscalData { get; set; }

            /// <summary>
            /// Gets or sets the receipt.
            /// </summary>
            [XmlElement(ElementName = EfstaSimpleReceiptElementName)]
            public Receipt Receipt { get; set; }
        }
    }
}
