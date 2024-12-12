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
        /// Represent bar code types.
        /// </summary>
        [DataContract]
        public enum BarCodeType
        {
            /// <summary>
            /// EAN8 type.
            /// </summary>
            [EnumMember(Value = "EAN8")]
            Ean8,

            /// <summary>
            /// EAN13 type.
            /// </summary>
            [EnumMember(Value = "EAN13")]
            Ean13,

            /// <summary>
            /// UPCE type.
            /// </summary>
            [EnumMember(Value = "UPCE")]
            Upce,

            /// <summary>
            /// CODE39 type.
            /// </summary>
            [EnumMember(Value = "CODE39")]
            Code39,

            /// <summary>
            /// CODE93 type.
            /// </summary>
            [EnumMember(Value = "CODE93")]
            Code93,

            /// <summary>
            /// CODE128 type.
            /// </summary>
            [EnumMember(Value = "CODE128")]
            Code128,

            /// <summary>
            /// CODEBAR type.
            /// </summary>
            [EnumMember(Value = "CODEBAR")]
            Codebar,

            /// <summary>
            /// ITF type.
            /// </summary>
            [EnumMember(Value = "ITF")]
            Itf,

            /// <summary>
            /// ITF14 type.
            /// </summary>
            [EnumMember(Value = "ITF14")]
            Itf14,

            /// <summary>
            /// GS1_128 type.
            /// </summary>
            [EnumMember(Value = "GS1_128")]
            Gs1_128,

            /// <summary>
            /// PDF417 type.
            /// </summary>
            [EnumMember(Value = "PDF417")]
            Pdf417,

            /// <summary>
            /// QR code type.
            /// </summary>
            [EnumMember(Value = "QR")]
            Qr,

            /// <summary>
            ///  CODE39_Extended type.
            /// </summary>
            [EnumMember(Value = "CODE39_Extended")]
            Code39_Extended,
        }
    }
}
