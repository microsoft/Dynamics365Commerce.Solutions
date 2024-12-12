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
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents item text.
        /// </summary>
        [DataContract]
        public class TextItem : BaseItem
        {
            /// <summary>
            /// Gets type of the item.
            /// </summary>
            [DataMember(Name = "type")]
            public override string Type { get { return "text"; } }

            /// <summary>
            /// Gets or sets text value.
            /// </summary>
            [DataMember(Name = "text")]
            public string TextValue { get; set; }

            /// <summary>
            /// Gets or sets alignment.
            /// </summary>
            [IgnoreDataMember]
            public AlignmentType Alignment { get; set; }

            /// <summary>
            /// Gets or sets alignment as a string.
            /// </summary>
            [DataMember(Name = "alignment")]
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
            /// Gets or sets wrap.
            /// </summary>
            [IgnoreDataMember]
            public WrapType Wrap { get; set; }

            /// <summary>
            /// Gets or sets wrap type of the text.
            /// </summary>
            [DataMember(Name = "wrap")]
            public string WrapStringValue
            {
                get
                {
                    return EnumMemberSerializer.Serialize(this.Wrap);
                }

                set
                {
                    this.Wrap = EnumMemberSerializer.Deserialize<WrapType>(value);
                }
            }

            /// <summary>
            /// Gets or sets font name.
            /// </summary>
            [DataMember(Name = "font")]
            public int Font { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether font width is double size.
            /// </summary>
            [DataMember(Name = "doubleWidth")]
            public bool DoubleWidth { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether font height is double size.
            /// </summary>
            [DataMember(Name = "doubleHeight")]
            public bool DoubleHeight { get; set; }
        }
    }
}
