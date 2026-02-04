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
        /// The sales transaction fiscal data.
        /// </summary>
        [Serializable]
        public class FiscalData
        {
            private const string TransactionIdAttributeName = "TID";
            private const string FiscalSignatureElementName = "Code";
            private const string FiscalLinkElementName = "Link";
            private const string QRCodeElementName = "QR";
            private const string FiscalTagElementName = "Tag";
            private const string StartDateTimeAttributeName = "StartD";

            /// <summary>
            /// Gets or sets the fiscal transaction id.
            /// </summary>
            [XmlAttribute(TransactionIdAttributeName)]
            public string TransactionId { get; set; }

            /// <summary>
            /// Gets or sets the fiscal signature.
            /// </summary>
            [XmlElement(ElementName = FiscalSignatureElementName)]
            public string FiscalSignature { get; set; }

            /// <summary>
            /// Gets or sets the fiscal link.
            /// </summary>
            [XmlElement(ElementName = FiscalLinkElementName)]
            public string FiscalLink { get; set; }

            /// <summary>
            /// Gets or sets the fiscal QR code.
            /// </summary>
            [XmlElement(ElementName = QRCodeElementName)]
            public string FiscalQRCode { get; set; }

            /// <summary>
            /// Gets or sets the fiscal tags.
            /// </summary>
            [XmlElement(ElementName = FiscalTagElementName)]
            public FiscalTag[] FiscalTags { get; set; }

            /// <summary>
            /// Gets or sets the fiscal transaction start DateTime.
            /// </summary>
            [XmlAttribute(StartDateTimeAttributeName)]
            public string StartDateTime { get; set; }
        }
    }
}
