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
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Xml.Linq;

        /// <summary>
        /// Reads the settings from fiscal document provider configuration.
        /// </summary>
        public static class ConfigurationController
        {
            private const string MappingToIncorrectVATGroupErrorMessage = "The tax code {0} is mapped to the control unit VAT group number {1}, which is not within a valid range.";
            private const int MinSupportedCleanCashVATId = 1;
            private const int MaxSupportedCleanCashVATId = 4;

            /// <summary>
            /// Gets the tax codes mapping from the functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The tax codes mapping.</returns>
            public static TaxCodesMapping GetSupportedTaxCodes(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string taxCodeSetting = GetStringValueElement(functionalityProfile.DocumentProviderProperties,
                        ConfigurationElementConstants.FiscalServiceDataMappingInfo,
                        ConfigurationElementConstants.TaxCodesMapping).Value;

                Dictionary<string, int> supportedTaxCodes =
                    taxCodeSetting.Split(';')
                        .Select(v => v.Split(':'))
                        .ToDictionary(l => l[1].Trim(), l => Convert.ToInt32(l[0].Trim()));

                if (supportedTaxCodes.Any(c => c.Value < MinSupportedCleanCashVATId && c.Value > MaxSupportedCleanCashVATId))
                {
                    throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ValueOutOfRange, MappingToIncorrectVATGroupErrorMessage);

                }
                return new TaxCodesMapping(supportedTaxCodes);
            }

            /// <summary>
            /// Find the StringValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The StoredStringValue element.</returns>
            private static XElement GetStringValueElement(string document, string namespaceValue, string nameValue)
            {
                return XElement.Parse(document)
                    .Descendants(ConfigurationElementConstants.PropertyElement)
                    .Single(a =>
                            a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue
                         && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue)
                    .Descendants(ConfigurationElementConstants.StringValueElement).First();
            }
        }
    }
}