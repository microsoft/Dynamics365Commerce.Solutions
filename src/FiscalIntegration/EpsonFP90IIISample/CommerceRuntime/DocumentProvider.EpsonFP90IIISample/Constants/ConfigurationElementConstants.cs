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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants
    {
        /// <summary>
        /// Contains constants of elements for fiscal document provider configuration.
        /// </summary>
        /// <remark>
        /// This is defined by a third-party library, or by the tax authority, etc.
        /// </remark>
        public static class ConfigurationElementConstants
        {
            /// <summary>
            /// The name of configuration version element.
            /// </summary>
            public const string ConfigurationVersion = "ConfigurationVersion";

            /// <summary>
            /// The name of EnableFreeOfChargeItems property.
            /// </summary>
            public const string EnableFreeOfChargeItems = "EnableFreeOfChargeItems";

            /// <summary>
            /// The inner text of Namespace element for DocumentProviderGeneralInfo.
            /// </summary>
            public const string DocumentProviderGeneralInfo = "DocumentProviderGeneralInfo";

            /// <summary>
            /// The inner text of Name element for DepositPaymentType property.
            /// </summary>
            internal const string DepositPaymentType = "DepositPaymentType";

            /// <summary>
            /// The inner text of Namespace element for FiscalServiceDataMappingInfo.
            /// </summary>
            internal const string FiscalServiceDataMappingInfo = "FiscalServiceDataMappingInfo";

            /// <summary>
            /// The Name element.
            /// </summary>
            internal const string NameElement = "Name";

            /// <summary>
            /// The Namespace element.
            /// </summary>
            internal const string NamespaceElement = "Namespace";

            /// <summary>
            /// The ConfigurationProperty element.
            /// </summary>
            internal const string PropertyElement = "ConfigurationProperty";

            /// <summary>
            /// The inner text of Name element for ReceiptNumberBarcodeType property.
            /// </summary>
            internal const string ReceiptNumberBarcodeType = "ReceiptNumberBarcodeType";

            /// <summary>
            /// The ConfigurationProperties element.
            /// </summary>
            internal const string RootElement = "ConfigurationProperties";

            /// <summary>
            /// The StringValue element.
            /// </summary>
            internal const string StringValueElement = "StringValue";

            /// <summary>
            /// The inner text of Name element for PaymentTypeMapping property.
            /// </summary>
            internal const string PaymentTypeMapping = "PaymentTypeMapping";

            /// <summary>
            /// The inner text of Name element for TenderTypeMapping property.
            /// </summary>
            internal const string TenderTypeMapping = "TenderTypeMapping";

            /// <summary>
            /// The inner text of Name element for VATRatesMapping property.
            /// </summary>
            internal const string VATRatesMapping = "VATRatesMapping";

            /// <summary>
            /// The inner text of Name element for FiscalPrinterDepartmentMapping property.
            /// </summary>
            internal const string FiscalPrinterDepartmentMapping = "FiscalPrinterDepartmentMapping";

            /// <summary>
            /// The inner text of Name element for VATExemptNatureForGiftCard property.
            /// </summary>
            internal const string VATExemptNatureForGiftCard = "VATExemptNatureForGiftCard";

            /// <summary>
            /// The BooleanValue element.
            /// </summary>
            internal const string BooleanValueElement = "BooleanValue";

            /// <summary>
            /// The IntegerValue element.
            /// </summary>
            internal const string IntegerValueElement = "IntegerValue";

            /// <summary>
            /// The inner text of Boolean element for PrintFiscalDatatInReceiptHeader property.
            /// </summary>
            internal const string PrintFiscalDataInReceiptHeader = "PrintFiscalDataInReceiptHeader";

            /// <summary>
            /// The inner text of Name element for ReturnOriginInfoCode property.
            /// </summary>
            internal const string ReturnOriginInfoCode = "ReturnOriginInfoCode";

            /// <summary>
            /// The inner text of Name element for OriginalSalesDateInfoCode property.
            /// </summary>
            internal const string OriginalSalesDateInfoCode = "OriginalSalesDateInfoCode";

            /// <summary>
            /// The inner text of Name element for ReturnOriginMapping property.
            /// </summary>
            internal const string ReturnOriginMapping = "ReturnOriginMapping";
        }
    }
}
