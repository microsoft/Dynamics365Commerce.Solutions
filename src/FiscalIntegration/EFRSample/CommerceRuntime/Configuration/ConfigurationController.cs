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
    namespace CommerceRuntime.DocumentProvider.EFRSample
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.Linq;
        using System.Xml.Linq;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Constants;

        /// <summary>
        /// Reads the settings from fiscal document provider configuration.
        /// </summary>
        internal static class ConfigurationController
        {
            /// <summary>
            /// Gets the tax rates mapping from the functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The tax rates mapping.</returns>
            public static TaxRatesMapping GetSupportedTaxRates(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string taxRateSetting = GetStringValueElement(functionalityProfile.DocumentProviderProperties,
                        ConfigurationElementConstants.FiscalServiceDataMappingInfo,
                        ConfigurationElementConstants.TaxRatesMapping)
                    .Value;

                Dictionary<decimal, string> supportedTaxRates =
                    taxRateSetting.Split(';')
                        .Select(v => v.Split(':'))
                        .ToDictionary(l => Convert.ToDecimal(l[1].Trim(), CultureInfo.InvariantCulture), l => l[0].Trim());

                return new TaxRatesMapping(supportedTaxRates);
            }

            /// <summary>
            /// Gets default tax group.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The default tax string.</returns>
            public static string GetDefaultTaxGroup(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string defaultTaxGroup = GetStringValueElement(functionalityProfile.DocumentProviderProperties,
                        ConfigurationElementConstants.FiscalServiceDataMappingInfo,
                        ConfigurationElementConstants.DefaultTaxGroup)
                    .Value;

                if (string.IsNullOrEmpty(defaultTaxGroup))
                {
                    ThrowValueReuiredException(defaultTaxGroup);
                }

                return defaultTaxGroup;
            }

            /// <summary>
            /// Gets deposit tax group.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The deposit tax group string.</returns>
            public static string GetDepositTaxGroup(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string depositTaxGroup = GetStringValueElement(functionalityProfile.DocumentProviderProperties,
                        ConfigurationElementConstants.FiscalServiceDataMappingInfo,
                        ConfigurationElementConstants.DepositTaxGroup)
                    .Value;

                if (string.IsNullOrEmpty(depositTaxGroup))
                {
                    ThrowValueReuiredException(depositTaxGroup);
                }

                return depositTaxGroup;
            }

            /// <summary>
            /// Gets the tender type mapping from the functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The tender type mapping.</returns>
            public static TenderTypeMapping GetSupportedTenderTypes(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string tenderTypeSetting = GetStringValueElement(functionalityProfile.DocumentProviderProperties,
                        ConfigurationElementConstants.FiscalServiceDataMappingInfo,
                        ConfigurationElementConstants.TenderTypeMapping)
                    .Value;

                Dictionary<string, string> supportedTenderTypes =
                    tenderTypeSetting.Split(';')
                        .Select(v => v.Split(':'))
                        .ToDictionary(l => l[0].Trim(), l => l[1].Trim());

                return new TenderTypeMapping(supportedTenderTypes);
            }

            /// <summary>
            /// Gets tender type mapping.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The tender type mapping.</returns>
            public static TenderTypeMapping GetTenderTypeMapping(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                ThrowIf.NullOrWhiteSpace(functionalityProfile.ProfileId, nameof(functionalityProfile.ProfileId));

                var tenderTypeMappings = new Dictionary<string, TenderTypeMapping>();

                if (!tenderTypeMappings.ContainsKey(functionalityProfile.ProfileId))
                {
                    tenderTypeMappings.Add(
                        functionalityProfile.ProfileId,
                        ConfigurationController.GetSupportedTenderTypes(functionalityProfile));
                }

                return tenderTypeMappings[functionalityProfile.ProfileId];
            }

            /// <summary>
            /// Gets deposit tax group.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The deposit tax group string.</returns>
            public static string GetExemptTaxGroup(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string exemptTaxGroup = GetStringValueElement(functionalityProfile.DocumentProviderProperties,
                        ConfigurationElementConstants.FiscalServiceDataMappingInfo,
                        ConfigurationElementConstants.ExemptTaxGroup).Value;

                if (string.IsNullOrEmpty(exemptTaxGroup))
                {
                    ThrowValueReuiredException(exemptTaxGroup);
                }

                return exemptTaxGroup;
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

            /// <summary>
            /// Throughs DataValidationException if a required field is empty.
            /// </summary>
            /// <param name="requiredField">The required field.</param>
            private static void ThrowValueReuiredException(string requiredField) {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound, string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} is empty", nameof(requiredField)));
            }

            /// <summary>
            /// Parses the "Print customer data in receipt" settings in fucntionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The value of ParsePrintCustomerDataInReceipt field or true if field is empty or doesn`t exist.</returns>
            public static bool ParsePrintCustomerDataInReceipt(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                return GetBooleanValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.PrintCustomerDataInReceipt);
            }

            /// <summary>
            /// Find the BooleanValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The StringValue element.</returns>
            public static bool GetBooleanValueElement(string document, string namespaceValue, string nameValue)
            {
                XElement xElement = XElement.Parse(document);
                bool result;

                if (PropertyExists(xElement, namespaceValue, nameValue))
                {
                    string booleanValue = xElement.Descendants(ConfigurationElementConstants.PropertyElement)
                        .Single(element => ElementExists(element, namespaceValue, nameValue))
                        .Descendants(ConfigurationElementConstants.BooleanValueElement).First().Value;

                    if (Boolean.TryParse(booleanValue, out result))
                    {
                        return result;
                    }
                }

                return true;
            }

            /// <summary>
            /// Gets whether the property for specific namespace and name exists or not.
            /// </summary>
            /// <param name="element">The element.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>True if the element exists, else false.</returns>
            private static bool PropertyExists(XElement element, string namespaceValue, string nameValue)
            {
                IEnumerable<XElement> propertyElements =
                    from item in element.Descendants(ConfigurationElementConstants.PropertyElement)
                    where item.Element("Namespace").Value == namespaceValue && item.Element("Name").Value == nameValue
                    select item;

                return propertyElements.Any();
            }

            /// <summary>
            /// Gets whether the element for specific namespace and name exists or not.
            /// </summary>
            /// <param name="element">The element.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>True if the element exists, else false.</returns>
            private static bool ElementExists(XElement element, string namespaceValue, string nameValue)
            {
                return element.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue
                    && element.Element(ConfigurationElementConstants.NameElement).Value == nameValue;
            }
        }
    }
}
