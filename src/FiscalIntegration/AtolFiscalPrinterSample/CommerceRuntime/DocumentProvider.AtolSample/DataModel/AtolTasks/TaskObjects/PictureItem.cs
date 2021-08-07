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
        /// Represents picture item.
        /// </summary>
        [DataContract]
        public class PictureItem : BaseItem
        {
            /// <summary>
            /// Gets type of the item.
            /// </summary>
            [DataMember(Name = "type")]
            public override string Type { get { return "pixels"; } }

            /// <summary>
            /// Gets or sets image.
            /// </summary>
            [DataMember(Name = "pixels")]
            public string Pixels { get; set; }

            /// <summary>
            /// Gets or sets image width.
            /// </summary>
            [DataMember(Name = "width")]
            public int Width { get; set; }

            /// <summary>
            /// Gets or sets image scale.
            /// </summary>
            [DataMember(Name = "scale")]
            public int Scale { get; set; }

            /// <summary>
            /// Gets or sets image alignment.
            /// </summary>
            [IgnoreDataMember]
            public AlignmentType Alignment { get; set; }

            /// <summary>
            /// Gets or sets image alignment text.
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
        }
    }
}