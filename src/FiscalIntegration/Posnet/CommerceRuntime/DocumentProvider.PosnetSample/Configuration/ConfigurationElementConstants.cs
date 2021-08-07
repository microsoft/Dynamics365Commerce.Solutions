namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.Configuration
    {
        /// <summary>
        /// Contains constants of elements for fiscal document provider configuration.
        /// </summary>
        /// <remark>
        /// This is defined by a third-party library, or by the tax authority, etc.
        /// </remark>
        internal static class ConfigurationElementConstants
        {
            /// <summary>
            /// The inner text of Namespace element for FiscalServiceDataMappingInfo.
            /// </summary>
            internal const string FiscalPrinterDataMappingInfo = "FiscalServiceDataMappingInfo";

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
            /// The ConfigurationProperties element.
            /// </summary>
            internal const string RootElement = "ConfigurationProperties";

            /// <summary>
            /// The StringValue element.
            /// </summary>
            internal const string StringValueElement = "StringValue";

            /// <summary>
            /// The IntegerValue element.
            /// </summary>
            internal const string IntegerValueElement = "IntegerValue";

            /// <summary>
            /// The inner text of Name element for TenderTypeMapping property.
            /// </summary>
            internal const string TenderTypeMapping = "TenderTypeMapping";

            /// <summary>
            /// The inner text of Name element for VATRatesMapping property.
            /// </summary>
            internal const string VATRatesMapping = "VATRatesMapping";

            /// <summary>
            /// The inner text of Name element for DepositPaymentType property.
            /// </summary>
            internal const string DepositPaymentType = "DepositPaymentType";
        }
    }
}
