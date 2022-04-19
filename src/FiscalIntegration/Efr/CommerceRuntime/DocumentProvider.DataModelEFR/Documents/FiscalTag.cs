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
        /// The sales transaction fiscal tag.
        /// </summary>
        [Serializable]
        public class FiscalTag
        {
            private const string FieldNameAttributeName = "Name";
            private const string FieldValueAttributeName = "Value";
            private const string FieldLabelAttributeName = "Label";

            /// <summary>
            /// Gets or sets the field name.
            /// </summary>
            [XmlAttribute(AttributeName = FieldNameAttributeName)]
            public string FieldName { get; set; }

            /// <summary>
            /// Gets or sets the field value.
            /// </summary>
            [XmlAttribute(AttributeName = FieldValueAttributeName)]
            public string FieldValue { get; set; }

            /// <summary>
            /// Gets or sets the field label.
            /// </summary>
            [XmlAttribute(AttributeName = FieldLabelAttributeName)]
            public string FieldLabel { get; set; }
        }
    }
}
