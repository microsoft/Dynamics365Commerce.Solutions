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
        /// Payment methods supported by the device.
        /// </summary>
        [DataContract]
        public enum PaymentMethodType
        {
            /// <summary>
            /// Cash payment method.
            /// </summary>
            [EnumMember(Value = "cash")]
            Cash = 0,

            /// <summary>
            /// Electronically payment method.
            /// </summary>
            [EnumMember(Value = "electronically")]
            Electronically = 1,

            /// <summary>
            /// Prepaid payment method.
            /// </summary>
            [EnumMember(Value = "prepaid")]
            Prepaid = 2,

            /// <summary>
            /// Credit payment method.
            /// </summary>
            [EnumMember(Value = "credit")]
            Credit = 3,

            /// <summary>
            /// Other payment method.
            /// </summary>
            [EnumMember(Value = "other")]
            Other = 4,
        }
    }
}
