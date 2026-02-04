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
        /// Payment methods supported by items on receipt.
        /// </summary>
        [DataContract]
        [DefaultValue(FullPrepayment)]
        public enum PositionPaymentMethod
        {
            /// <summary>
            /// Full prepayment.
            /// </summary>
            [EnumMember(Value = "fullPrepayment")]
            FullPrepayment,

            /// <summary>
            /// Prepeyment.
            /// </summary>
            [EnumMember(Value = "prepayment")]
            Prepayment,

            /// <summary>
            /// Advanced.
            /// </summary>
            [EnumMember(Value = "advance")]
            Advance,

            /// <summary>
            /// Full payment.
            /// </summary>
            [EnumMember(Value = "fullPayment")]
            FullPayment,

            /// <summary>
            /// Partial payment.
            /// </summary>
            [EnumMember(Value = "partialPayment")]
            PartialPayment,

            /// <summary>
            /// Credit.
            /// </summary>
            [EnumMember(Value = "credit")]
            Credit,

            /// <summary>
            /// Credit payment.
            /// </summary>
            [EnumMember(Value = "creditPayment")]
            CreditPayment,
        }
    }
}
