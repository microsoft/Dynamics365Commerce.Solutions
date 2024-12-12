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
        /// Represents image items from memory.
        /// </summary>
        [DataContract]
        public class PictureFromMemoryItem
        {
            /// <summary>
            /// Gets type of the item.
            /// </summary>
            [DataMember(Name = "type")]
            public string Type { get { return "pictureFromMemory"; } private set { } }

            /// <summary>
            /// Gets or sets image number.
            /// </summary>
            [DataMember(Name ="pictureNumber")]
            public int PictureNumber { get; set; }

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
