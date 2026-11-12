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
        using System.ComponentModel;
        using System.Xml.Serialization;

        /// <summary>
        /// The receipt customer.
        /// </summary>
        [Serializable]
        public class ReceiptCustomer
        {
            private const string CustomerNumberAttributeName = "CN";
            private const string CustomerNameAttributeName = "Nam";
            private const string AddressAttributeName = "Adr";
            private const string VatNumberAttributeName = "TaxId";

            /// <summary>
            /// Gets or sets the customer number.
            /// </summary>
            [XmlAttribute(AttributeName = CustomerNumberAttributeName)]
            [DefaultValue("")]
            public string CustomerNumber { get; set; }

            /// <summary>
            /// Gets or sets the customer or company name.
            /// </summary>
            [XmlAttribute(AttributeName = CustomerNameAttributeName)]
            [DefaultValue("")]
            public string CustomerName { get; set; }

            /// <summary>
            /// Gets or sets the customer or company address within city.
            /// </summary>
            [XmlAttribute(AttributeName = AddressAttributeName)]
            [DefaultValue("")]
            public string Address { get; set; }

            /// <summary>
            /// Gets or sets the customer or company VAT number including eventual country/region prefix.
            /// </summary>
            [XmlAttribute(AttributeName = VatNumberAttributeName)]
            [DefaultValue("")]
            public string VatNumber { get; set; }
        }
    }
}
