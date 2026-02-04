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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample
    {
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Linq;
        using System.Runtime.Serialization;
        using System.Runtime.Serialization.Json;
        using System.Text;
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Reads the settings in fiscal document provider configuration.
        /// </summary>
        public static class ConfigurationController
        {
            /// <summary>
            /// Parses the deposit payment type in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The deposit payment type.</returns>
            public static string ParseDepositPaymentType(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string depositPaymentType = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.DepositPaymentType).Value;

                if (string.IsNullOrEmpty(depositPaymentType))
                {
                    throw new Exception("The payment method for the deposit payment is missing in the document provider configuration.");
                }

                return depositPaymentType;
            }

            /// <summary>
            /// Parses the receipt number barcode type in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The receipt number barcode type.</returns>
            public static string ParseReceiptNumberBarcodeType(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string barcodeType = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.ReceiptNumberBarcodeType).Value;

                return barcodeType;
            }

            /// <summary>
            /// Parses the VAT exempt nature for gift card in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The VAT exempt nature.</returns>
            public static string ParseVATExemptNatureForGiftCard(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string vatTExemptNature = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.VATExemptNatureForGiftCard).Value;

                return vatTExemptNature;
            }

            /// <summary>
            /// Parses the supported payment types from payment types mapping in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <param name="depositPaymentMethod">Mapping values for deposit payment method.</param>
            /// <returns>The supported payment types.</returns>
            public static Dictionary<string, PaymentMethod> ParseSupportedPaymentTypeMappings(FiscalIntegrationFunctionalityProfile functionalityProfile, out PaymentMethod depositPaymentMethod)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                depositPaymentMethod = null;
                if (IsPropertyExisted(XElement.Parse(functionalityProfile.DocumentProviderProperties), ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.PaymentTypeMapping))
                {
                    Dictionary<string, PaymentMethod> supportedPaymentMethods = new Dictionary<string, PaymentMethod>();
                    string paymentTypeString = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.PaymentTypeMapping).Value;
                    PaymentMethodMapping root = DeserializeJson<PaymentMethodMapping>(paymentTypeString);
                    if (root != null)
                    {
                        foreach (var item in root.PaymentMethods)
                        {
                            supportedPaymentMethods.Add(item.StorePaymentMethod, item);
                        }
                        depositPaymentMethod = root.DepositPaymentMethod;
                    }
                    return supportedPaymentMethods;
                }
                return null;
            }

            /// <summary>
            /// Parses the supported tender types form tender types mapping in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The supported tender types.</returns>
            public static Dictionary<string, int> ParseSupportedTenderTypeMappings(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string tenderTypeMapping = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.TenderTypeMapping).Value;

                Dictionary<string, int> supportedTenderType =
                    tenderTypeMapping.Split(';')
                        .Select(v => v.Split(':'))
                        .ToDictionary(l => l[0].Trim(), l => Convert.ToInt32(l[1].Trim()));

                return supportedTenderType;
            }

            /// <summary>
            /// Parses the supported VAT rates form VAT Rates settings in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The supported VAT rates.</returns>
            public static Dictionary<int, string> ParseSupportedVATRates(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string vatRateSetting = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.VATRatesMapping).Value;

                Dictionary<int, string> supportedVATRates =
                    vatRateSetting.Split(';')
                        .Select(v => v.Split(':'))
                        .ToDictionary(l => SalesOrderHelper.ConvertSalesTaxRateToInt(l[1].Trim()), l => l[0].Trim());

                return supportedVATRates;
            }

            /// <summary>
            /// Parses the "Print fiscal data in receipt header" settings in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The value of PrintFiscalDatatInReceiptHeader field or true if field is empty or doesn`t exist.</returns>
            public static bool ParsePrintFiscalDatatInReceiptHeader(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                return GetBooleanValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.PrintFiscalDataInReceiptHeader);

            }

            /// <summary>
            /// Parses the supported department numbers from departments mapping in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>The supported department numbers.</returns>
            public static Dictionary<ExtendedVATRate, string> ParseSupportedDepartmentNumbersMappings(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string departmentInfoString = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.FiscalPrinterDepartmentMapping).Value;
                DepartmentMappings root = DeserializeJson<DepartmentMappings>(departmentInfoString);
                var supportedDepartmentNumbers = root.Departments.ToDictionary(item => new ExtendedVATRate(Convert.ToInt32(item.VATRate), item.VATExemptNature, item.ProductType), item => item.DepartmentNumber);

                return supportedDepartmentNumbers;
            }

            /// <summary>
            /// Find the StringValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The StringValue element.</returns>
            public static XElement GetStringValueElement(string document, string namespaceValue, string nameValue)
            {
                XElement xElement = XElement.Parse(document);

                return xElement.Descendants(ConfigurationElementConstants.PropertyElement)
                    .Single(element => IsElementExisted(element, namespaceValue, nameValue)).Descendants(ConfigurationElementConstants.StringValueElement).First();
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

                if (IsPropertyExisted(xElement, namespaceValue, nameValue))
                {
                    string booleanValue = xElement.Descendants(ConfigurationElementConstants.PropertyElement)
                        .Single(element => IsElementExisted(element, namespaceValue, nameValue)).Descendants(ConfigurationElementConstants.BooleanValueElement).First().Value;

                    if (Boolean.TryParse(booleanValue, out result))
                    {
                        return result;
                    }
                }

                return nameValue == ConfigurationElementConstants.PrintFiscalDataInReceiptHeader? true : false;
            }

            /// <summary>
            /// Find the IntegerValue element for specific name and namespace of the fiscal document provider configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider configuration in functionality profile.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>The IntegerValue element.</returns>
            public static int? GetIntegerValueElement(string document, string namespaceValue, string nameValue)
             {
                XElement xElement = XElement.Parse(document);

                if (IsPropertyExisted(xElement, namespaceValue, nameValue))
                {
                    var integerValue = xElement.Descendants(ConfigurationElementConstants.PropertyElement)
                        .Single(element => IsElementExisted(element, namespaceValue, nameValue)).Descendants(ConfigurationElementConstants.IntegerValueElement).First().Value;

                    if (Int32.TryParse(integerValue, out var result))
                    {
                        return result;
                    }
                }

                return null;
             }

            /// <summary>
            /// Gets the configuration version.
            /// </summary>
            /// <param name="functionalityProfile">The retail fiscal integration functionality profile.</param>
            /// <returns>The configuration version enum.</returns>
            public static ConfigurationVersion GetConfigurationVersion(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                var configurationVersion = GetIntegerValueElement(functionalityProfile.DocumentProviderProperties,
                    ConfigurationElementConstants.DocumentProviderGeneralInfo,
                    ConfigurationElementConstants.ConfigurationVersion);

                switch (configurationVersion)
                {
                    case null:
                    case 0:
                    case 1:
                        return ConfigurationVersion.RT;
                    default:
                        return ConfigurationVersion.RT2;
                }
            }

            /// <summary>
            /// Deserializes JSON.
            /// </summary>
            /// <typeparam name="T">The type to instantiate.</typeparam>
            /// <param name="json">JSON string.</param>
            /// <returns>Strongly typed version of the deserialized JSON.</returns>
            public static T DeserializeJson<T>(string json)
            {
                if (string.IsNullOrEmpty(json))
                {
                    return default(T);
                }

                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    try
                    {
                        var serializer = new DataContractJsonSerializer(typeof(T));
                        return (T)serializer.ReadObject(memoryStream);
                    }
                    catch (SerializationException)
                    {
                        return default(T);
                    }
                }
            }

            /// <summary>
            /// Parses EnableFreeOfChargeItems boolean value in the functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The retail fiscal integration functionality profile.</param>
            /// <returns>EnableFreeOfChargeItems property boolean value.</returns>
            public static bool ParseEnableFreeOfChargeItemsProperty(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                return GetBooleanValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.EnableFreeOfChargeItems);
            }

            /// <summary>
            /// Parses return origin info code string value in the functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The retail fiscal integration functionality profile.</param>
            /// <returns>Return origin info code property string value.</returns>
            public static string GetReturnOriginInfoCodeValue(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                var returnOriginInfoCode = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.ReturnOriginInfoCode).Value;

                if (string.IsNullOrEmpty(returnOriginInfoCode))
                {
                    throw new EpsonFP90IIIDocumentProviderConfigurationException($"Info code for return origin is not specified in the connector functional profile {functionalityProfile.ProfileId} or is incorrect.");
                }

                return returnOriginInfoCode;
            }

            /// <summary>
            /// Parses original sales date info code string value in the functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The retail fiscal integration functionality profile.</param>
            /// <returns>Original sales date info code property string value.</returns>
            public static string GetOriginalSalesDateInfoCodeValue(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));
                var originalSaleDateInfoCode = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.OriginalSalesDateInfoCode).Value;

                if (string.IsNullOrEmpty(originalSaleDateInfoCode))
                {
                    throw new EpsonFP90IIIDocumentProviderConfigurationException($"Info code for original sales date is not specified in the connector functional profile {functionalityProfile.ProfileId} or is incorrect.");
                }

                return originalSaleDateInfoCode;
            }

            /// <summary>
            /// Parses return origin mapping in functionality profile.
            /// </summary>
            /// <param name="functionalityProfile">The functionality profile.</param>
            /// <returns>Return origin mapping property value.</returns>
            public static ReturnOriginMapping ParseReturnOriginMappings(FiscalIntegrationFunctionalityProfile functionalityProfile)
            {
                ThrowIf.Null(functionalityProfile, nameof(functionalityProfile));

                string returnOriginsMappingsString = GetStringValueElement(functionalityProfile.DocumentProviderProperties, ConfigurationElementConstants.FiscalServiceDataMappingInfo, ConfigurationElementConstants.ReturnOriginMapping).Value;
                var returnOriginMapping = DeserializeJson<ReturnOriginMapping>(returnOriginsMappingsString);

                if (returnOriginMapping == null)
                {
                    throw new EpsonFP90IIIDocumentProviderConfigurationException($"Return origin mapping is not specified in the connector functional profile {functionalityProfile.ProfileId} or is incorrect.");
                }
                return returnOriginMapping;
            }

            /// <summary>
            /// Gets the property for specific namespace and checks if the name exists.
            /// </summary>
            /// <param name="element">The element.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>True if the element exists, otherwise returns false.</returns>
            private static bool IsPropertyExisted(XElement element, string namespaceValue, string nameValue)
            {
                IEnumerable<XElement> propertyElements = 
                    from item in element.Descendants(ConfigurationElementConstants.PropertyElement)
                    where item.Element("Namespace").Value == namespaceValue && item.Element("Name").Value == nameValue
                    select item;

                return propertyElements.Any();
            }

            /// <summary>
            /// Gets the element for specific namespace and checks if the name exists.
            /// </summary>
            /// <param name="element">The element.</param>
            /// <param name="namespaceValue">The value of namespace element.</param>
            /// <param name="nameValue">The value of name element.</param>
            /// <returns>True if the element exists, otherwise returns false.</returns>
            private static bool IsElementExisted(XElement element, string namespaceValue, string nameValue)
            {
                return element.Element(ConfigurationElementConstants.NamespaceElement)?.Value == namespaceValue
                    && element.Element(ConfigurationElementConstants.NameElement)?.Value == nameValue;
            }
        }
    }
}
