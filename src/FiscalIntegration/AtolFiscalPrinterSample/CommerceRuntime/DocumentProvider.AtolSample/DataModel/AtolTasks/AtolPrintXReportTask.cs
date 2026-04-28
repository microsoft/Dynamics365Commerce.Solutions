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
        /// Represent task to generate X-report on the printer.
        /// </summary>
        [DataContract]
        public class AtolPrintXReportTask
        {
            /// <summary>
            /// Gets type of the task.
            /// </summary>
            [DataMember(Name = "type")]
            public string Type { get { return AtolTaskTypes.XReport; } }

            /// <summary>
            /// Gets or sets operator of the task.
            /// </summary>
            [DataMember(Name ="operator", EmitDefaultValue = false)]
            public Operator Operator { get; set; }

            /// <summary>
            /// Gets or sets items to print in the header.
            /// </summary>
            [DataMember(Name = "preItems", EmitDefaultValue = false)]
            public ICollection<BaseItem> PreItems { get; set; }

            /// <summary>
            /// Gets or sets items to print in the bottom.
            /// </summary>
            [DataMember(Name = "postItems", EmitDefaultValue = false)]
            public ICollection<BaseItem> PostItems { get; set; }
        }
    }
}
