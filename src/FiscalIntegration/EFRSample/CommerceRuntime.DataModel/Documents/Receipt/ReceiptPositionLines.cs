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
        using System.Collections.Generic;
        using System.Xml.Serialization;

        /// <summary>
        /// The receipt position lines.
        /// </summary>
        [Serializable]
        public class ReceiptPositionLines
        {
            private const string ReceiptPositionsElementName = "Pos";
            private const string ReceiptPositionModifiersElementName = "Mod";
            private const string ReceiptPositionPrintLinesElementName = "Lin";

            /// <summary>
            /// Gets or sets the receipt positions array.
            /// </summary>
            [XmlElement(ElementName = ReceiptPositionsElementName)]
            public List<ReceiptPosition> ReceiptPositions { get; set; }

            /// <summary>
            /// Gets or sets the receipt position modifiers.
            /// </summary>
            [XmlElement(ElementName = ReceiptPositionModifiersElementName)]
            public List<ReceiptPositionModifier> ReceiptPositionModifiers { get; set; }

            /// <summary>
            /// Gets or sets the receipt position print lines.
            /// </summary>
            [XmlElement(ElementName = ReceiptPositionPrintLinesElementName)]
            public List<ReceiptPositionPrintLine> ReceiptPositionPrintLines { get; set; }
        }
    }
}
