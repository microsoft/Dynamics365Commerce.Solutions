namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.Configuration
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.Linq;
        using System.Xml.Linq;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        public static class ConfigurationController
        {
            /// <summary>
            /// Parses the supported tender types form tender types mapping in fucntionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The supported tender types.</returns>
            public static Dictionary<string, int> ParseSupportedTenderTypeMappings(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string tenderTypeMapping = GetValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalPrinterDataMappingInfo, ConfigurationElementConstants.TenderTypeMapping, ConfigurationElementConstants.StringValueElement).Value;

                Dictionary<string, int> supportedTenderType =
                    tenderTypeMapping.Split(';')
                        .Select(v => v.Split(':'))
                        .ToDictionary(l => l[0].Trim(), l => Convert.ToInt32(l[1].Trim()));

                return supportedTenderType;
            }

            /// <summary>
            /// Parses the supported VAT rates form VAT Rates settings in fucntionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The supported VAT rates as a collection of pairs {VATrate, RateId}.</returns>
            public static IEnumerable<Tuple<decimal, int>> ParseSupportedVATRates(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string vatRateSetting = GetValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalPrinterDataMappingInfo, ConfigurationElementConstants.VATRatesMapping, ConfigurationElementConstants.StringValueElement).Value;

                var supportedVATRates =
                    vatRateSetting.Split(';')
                        .Select(v => v.Split(':'))
                        .Select(l => new { Rate = Math.Round(Convert.ToDecimal(l[1].Trim(), CultureInfo.InvariantCulture), 2), Value = int.Parse(l[0].Trim()) })
                        .Select(p => Tuple.Create(p.Rate, p.Value))
                        .ToList();

                return supportedVATRates;
            }

            /// <summary>
            /// Find the StringValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namesSpaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The StoredStringValue element.</returns>
            public static XElement GetValueElement(string document, string namesSpaceValue, string nameValue, string typeOfValue)
            {
                XElement xElement = XElement.Parse(document);

                return xElement.Descendants(ConfigurationElementConstants.PropertyElement)
                              .Single(a => a.Element(ConfigurationElementConstants.NamespaceElement).Value == namesSpaceValue
                                   && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue).Descendants(typeOfValue).First();
            }

            /// <summary>
            /// Parses the deposit payment type in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The deposit payment type.</returns>
            public static int ParseDepositPaymentType(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string depositPaymentType = GetValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalPrinterDataMappingInfo, ConfigurationElementConstants.DepositPaymentType, ConfigurationElementConstants.IntegerValueElement).Value;

                if (string.IsNullOrEmpty(depositPaymentType))
                {
                    throw new Exception("The payment method for the deposit payment is missing in the document provider configuration.");
                }

                int printerPaymentType;

                if (!int.TryParse(depositPaymentType, out printerPaymentType))
                {
                    throw new Exception("The payment method must be an integer.");
                }

                return printerPaymentType;
            }
        }
    }
}
