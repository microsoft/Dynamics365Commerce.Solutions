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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.Configuration
    {
        /// <summary>
        /// Represents a list of configuration property names.
        /// </summary>
        public static class ConfigurationPropertiesNames
        {
            /// <summary>
            /// The inner text of Namespace element for DocumentProviderGeneralInfo.
            /// </summary>
            public const string NamespaceDocumentProviderGeneralInfo = "DocumentProviderGeneralInfo";

            /// <summary>
            /// The inner text of Namespace element for FiscalServiceDataMappingInfo.
            /// </summary>
            public const string NamespaceFiscalServiceDataMappingInfo = "FiscalServiceDataMappingInfo";

            /// <summary>
            /// The Namespace element.
            /// </summary>
            public const string PropertyGenerateCloseShiftReportElectronically = "GenerateCloseShiftReportElectronically";

            /// <summary>
            /// The Namespace element.
            /// </summary>
            public const string PropertyPaymentTypeMapping = "PaymentTypeMapping";
        }
    }
}
