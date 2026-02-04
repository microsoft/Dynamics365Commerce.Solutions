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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask
    {
        using System.Collections.Generic;
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents bar code item.
        /// </summary>
        [DataContract]
        public class BarcodeItem : BaseItem
        {
            /// <summary>
            /// Gets type of the item.
            /// </summary>
            [DataMember(Name ="type")]
            public override string Type { get { return "barcode"; } }

            /// <summary>
            /// Gets or sets barcode value.
            /// </summary>
            [DataMember(Name ="barcode")]
            public string BarcodeValue { get; set; }

            /// <summary>
            /// Gets or sets barcode type.
            /// </summary>
            [IgnoreDataMember]
            public BarCodeType BarcodeType { get; set; }

            /// <summary>
            /// Gets or sets barcode type text.
            /// </summary>
            [DataMember(Name ="barcodeType")]
            public string BarcodeTypeStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.BarcodeType);
                }

                set
                {
                    this.BarcodeType = EnumMemberSerializer.Deserialize<BarCodeType>(value);
                }
            }

            /// <summary>
            /// Gets or sets alignment type.
            /// </summary>
            [IgnoreDataMember]
            public AlignmentType Alignment { get; set; }

            /// <summary>
            /// Gets or sets alignment type text.
            /// </summary>
            [DataMember(Name="alignment", EmitDefaultValue = false)]
            public string AlignmentStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.Alignment);
                }

                set
                {
                    this.Alignment = EnumMemberSerializer.Deserialize<AlignmentType>(value);
                }
            }

            /// <summary>
            /// Gets or sets scaling factor.
            /// </summary>
            [DataMember(Name = "scale", EmitDefaultValue = false)]
            public int Scale { get; set; }

            /// <summary>
            /// Gets or sets height value.
            /// </summary>
            [DataMember(Name = "height")]
            public int Height { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether print text next to the barcode.
            /// </summary>
            [DataMember(Name = "printText", EmitDefaultValue = false)]
            public bool PrintText { get; set; }

            /// <summary>
            /// Gets or sets text to be printed next to the barcode.
            /// </summary>
            [DataMember(Name = "overlay", EmitDefaultValue = false)]
            public List<TextItem> Overlay { get; set; }
        }
    }
}
