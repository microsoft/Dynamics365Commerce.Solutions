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
        /// Receipt types supported by the device.
        /// </summary>
        [DataContract]
        public enum ReceiptType
        {
            /// <summary>
            /// Sell.
            /// </summary>
            [EnumMember(Value = "sell")]
            Sell,

            /// <summary>
            /// Buy.
            /// </summary>
            [EnumMember(Value = "buy")]
            Buy,

            /// <summary>
            /// Sell return.
            /// </summary>
            [EnumMember(Value = "sellReturn")]
            SellReturn,

            /// <summary>
            /// Buy return.
            /// </summary>
            [EnumMember(Value = "buyReturn")]
            BuyReturn,
        }
    }
}