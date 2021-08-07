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
        /// Represents user attribute item.
        /// </summary>
        [DataContract]
        public class UserAttributeItem : BaseItem
        {
            /// <summary>
            /// Gets type of the item.
            /// </summary>
            [DataMember(Name = "type")]
            public override string Type { get { return "userAttribute"; } }

            /// <summary>
            /// Gets or sets name of the attribute.
            /// </summary>
            [DataMember(Name="name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets value of the attribute.
            /// </summary>
            [DataMember(Name ="value")]
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether printing flag.
            /// </summary>
            [DataMember(Name ="print")]
            public bool Print { get; set; }
        }
    }
}
