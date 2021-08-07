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
    namespace CommerceRuntime.DocumentProvider.AtolSample.Handlers
    {
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Linq;
        using System.Threading.Tasks;
        using System.Xml;
        using System.Xml.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.DataModel.Configuration;
        using Contoso.CommerceRuntime.DocumentProvider.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Framework.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Represents an entry point for requests related to printer shift management tasks.
        /// </summary>
        public class ConfigurationDeserializerService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(DeserializeDocumentProviderSettingsDocumentProviderAtolRequest),
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">Th request.</param>
            /// <returns>The response.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Response response;
                if (request is DeserializeDocumentProviderSettingsDocumentProviderAtolRequest deserializeDocumentProviderSettingsDocumentProviderAtolRequest)
                {
                    response = this.DeserializeConnectorFunctiolityProfile(deserializeDocumentProviderSettingsDocumentProviderAtolRequest);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return response;
            }

            /// <summary>
            /// Deserialized document provider configuration properties.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private DeserializeDocumentProviderSettingsDocumentProviderAtolResponse DeserializeConnectorFunctiolityProfile(DeserializeDocumentProviderSettingsDocumentProviderAtolRequest request)
            {
                string properties = request.ConfigurationProperties.DocumentProviderProperties;
                var profile = new DocumentProviderSettings();
                List<ConfigurationProperty> configuration = this.Deserialize(properties);
                this.SetIsElectroniCloseShiftReportProperty(profile, configuration);
                this.SetPaymentTypeMapping(profile, configuration);
                return new DeserializeDocumentProviderSettingsDocumentProviderAtolResponse(profile);
            }

            /// <summary>
            /// Sets a value indicating whether the shift close shift report should be generated electronically only.
            /// </summary>
            /// <param name="profile">The document provider settings.</param>
            /// <param name="properties">The list of configuration properties.</param>
            private void SetIsElectroniCloseShiftReportProperty(DocumentProviderSettings profile, List<ConfigurationProperty> properties)
            {
                profile.GenerateCloseShiftReportElectronically = false;
                ConfigurationProperty property = properties?.Where(p => p.Name == ConfigurationPropertiesNames.PropertyGenerateCloseShiftReportElectronically && p.Namespace == ConfigurationPropertiesNames.NamespaceFiscalServiceDataMappingInfo).SingleOrDefault();
                if (property != null)
                {
                    bool result = false;
                    if (bool.TryParse(property.BooleanValue, out result))
                    {
                        profile.GenerateCloseShiftReportElectronically = result;
                    }
                }
            }

            /// <summary>
            /// Sets payment type mapping.
            /// </summary>
            /// <param name="profile">The document provider settings.</param>
            /// <param name="properties">The list of configuration properties.</param>
            private void SetPaymentTypeMapping(DocumentProviderSettings profile, List<ConfigurationProperty> properties)
            {
                profile.PaymentTypeMapping = new Dictionary<string, PaymentMethodType>();
                ConfigurationProperty property = properties?.Where(p => p.Name == ConfigurationPropertiesNames.PropertyPaymentTypeMapping && p.Namespace == ConfigurationPropertiesNames.NamespaceFiscalServiceDataMappingInfo).SingleOrDefault();
                if (property != null)
                {
                    if (JsonHelper.TryDeserialize(property.StringValue, out PaymentMethodMapping paymentMethodMapping))
                    {
                        foreach (var paymentMethod in paymentMethodMapping.PaymentMethods)
                        {
                            profile.PaymentTypeMapping.Add(paymentMethod.StorePaymentMethod, paymentMethod.AtolPaymentType);
                        }
                    }
                }
            }

            /// <summary>
            /// Deserializes xml string with properties.
            /// </summary>
            /// <param name="properties">Xml string with connector functionality profile properties.</param>
            /// <returns>List of the configuration properties.</returns>
            private List<ConfigurationProperty> Deserialize(string properties)
            {
                List<ConfigurationProperty> configuration = null;
                var xmlSerializer = new XmlSerializer(typeof(ConfigurationProperties));

                using (StringReader stringReader = new StringReader(properties))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        var configurationProperties = xmlSerializer.Deserialize(xmlReader) as ConfigurationProperties;
                        configuration = configurationProperties.ConfigurationPropertyList;
                    }
                }

                return configuration;
            }
        }
    }
}
