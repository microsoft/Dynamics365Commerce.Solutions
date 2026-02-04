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
    namespace CommerceRuntime.DocumentProvider.CleanCashSample
    {
        /// <summary>
        /// The elements for fiscal connector configuration.
        /// </summary>
        public static class ConfigurationElementConstants
        {
            /// <summary>
            /// The inner text of namespace element for data mapping.
            /// </summary>
            public const string FiscalServiceDataMappingInfo = "FiscalServiceDataMappingInfo";

            /// <summary>
            /// The name of the name element.
            /// </summary>
            public const string NameElement = "Name";

            /// <summary>
            /// The name of the namespace element.
            /// </summary>
            public const string NamespaceElement = "Namespace";

            /// <summary>
            /// The name of the property element.
            /// </summary>
            public const string PropertyElement = "ConfigurationProperty";

            /// <summary>
            /// The StringValue element.
            /// </summary>
            public const string StringValueElement = "StringValue";

            /// <summary>
            /// The inner text of name element for tax codes mapping property.
            /// </summary>
            public const string TaxCodesMapping = "TaxCodesMapping";
        }
    }
}