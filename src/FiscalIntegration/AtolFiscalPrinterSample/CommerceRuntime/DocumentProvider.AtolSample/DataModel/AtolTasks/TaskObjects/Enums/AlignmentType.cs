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
        using System.ComponentModel;
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents alignment types.
        /// </summary>
        [DataContract]
        [DefaultValue(Left)]
        public enum AlignmentType
        {
            /// <summary>
            /// Left alignment type.
            /// </summary>
            [EnumMember(Value ="left")]
            Left,

            /// <summary>
            /// Center alignment type.
            /// </summary>
            [EnumMember(Value ="center")]
            Center,

            /// <summary>
            /// Right alignment type.
            /// </summary>
            [EnumMember(Value ="right")]
            Right,
        }
    }
}
