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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask.TaskObjects
    {
        using System.Runtime.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;

        /// <summary>
        /// Represents additional attribute item.
        /// </summary>
        [DataContract]
        public class AdditionalAttributeItem : BaseItem
        {
            /// <summary>
            /// Gets type of the item.
            /// </summary>
            [DataMember(Name = "type")]
            public override string Type { get { return "additionalAttribute"; } }

            /// <summary>
            /// Gets or sets value of the attribute.
            /// </summary>
            [DataMember(Name = "value")]
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to print the details on the receipt or not.
            /// </summary>
            [DataMember(Name = "print")]
            public bool Print { get; set; }
        }
    }
}
