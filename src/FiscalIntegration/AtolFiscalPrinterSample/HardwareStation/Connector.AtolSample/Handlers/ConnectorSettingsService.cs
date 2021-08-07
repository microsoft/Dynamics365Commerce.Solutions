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
    namespace HardwareStation.Connector.AtolSample.Handlers
    {
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Linq;
        using System.Threading.Tasks;
        using System.Xml;
        using System.Xml.Serialization;
        using Contoso.HardwareStation.Connector.AtolSample.DataModel.Configuration;
        using Contoso.HardwareStation.Connector.AtolSample.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Service hadndles requests to work with printer configuration.
        /// </summary>
        public class ConnectorSettingsService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets supported types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes => new[]
            {
                typeof(GetConnectorConfigurationConnectorAtolRequest)
            };

            /// <summary>
            /// Represents an enty point for the request handler.
            /// </summary>
            /// <param name="request">Th request.</param>
            /// <returns>The response.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Type requestType = request.GetType();
                Response response;

                if (requestType == typeof(GetConnectorConfigurationConnectorAtolRequest))
                {
                    response = this.GetConnectorSettings((GetConnectorConfigurationConnectorAtolRequest)request);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return response;
            }

            /// <summary>
            /// Gets connector settings
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            private Response GetConnectorSettings(GetConnectorConfigurationConnectorAtolRequest request)
            {
                string properties = request.ConfigurationProperties.DeviceProperties;
                var profile = new ConnectorSettings();
                List<ConfigurationProperty> configuration = this.Deserialize(properties);
                this.SetPrintPreviousNotPrintedDocument(profile, configuration);
                this.SetComPort(profile, configuration);
                this.SetBaudRate(profile, configuration);
                return new GetConnectorConfigurationConnectorAtolResponse(profile);
            }
            
            /// <summary>
            /// Sets parameter to print previous not printed document.
            /// </summary>
            /// <param name="profile">Connector settings profile to modify.</param>
            /// <param name="properties">List of the properties.</param>
            private void SetPrintPreviousNotPrintedDocument(ConnectorSettings profile, List<ConfigurationProperty> properties)
            {
                profile.PrintPreviousNotPrintedDocument = false;
                ConfigurationProperty property = properties?.Where(p => p.Name == ConfigurationPropertiesNames.PropertyPrintPreviousNotPrintedDocument && p.Namespace == ConfigurationPropertiesNames.NamespaceConnectorSettingsInfo).SingleOrDefault();
                if (property != null)
                {
                    bool result = false;
                    if (Boolean.TryParse(property.BooleanValue, out result))
                    {
                        profile.PrintPreviousNotPrintedDocument = result;
                    }
                }
            }

            // <summary>
            /// Sets COM port parameter.
            /// </summary>
            /// <param name="profile">Connector settings profile to modify.</param>
            /// <param name="properties">List of the properties.</param>
            private void SetComPort(ConnectorSettings profile, List<ConfigurationProperty> properties)
            {
                profile.ComPort = string.Empty;
                ConfigurationProperty property = properties?.Where(p => p.Name == ConfigurationPropertiesNames.PropertyComPort && p.Namespace == ConfigurationPropertiesNames.NamespaceConnectorConnectionInfo).SingleOrDefault();
                if (property != null)
                {
                    string result = string.Empty;
                    profile.ComPort = property.StringValue;
                }
            }

            // <summary>
            /// Sets baud rate parameter.
            /// </summary>
            /// <param name="profile">Connector settings profile to modify.</param>
            /// <param name="properties">List of the propert
            private void SetBaudRate(ConnectorSettings profile, List<ConfigurationProperty> properties)
            {
                profile.BaudRate = 0;
                ConfigurationProperty property = properties?.Where(p => p.Name == ConfigurationPropertiesNames.PropertyBaudRate && p.Namespace == ConfigurationPropertiesNames.NamespaceConnectorConnectionInfo).SingleOrDefault();
                if (property != null)
                {
                    profile.BaudRate = Convert.ToInt32(property.IntegerValue);
                }
            }

            /// <summary>
            /// Tries to deserialize XML string.
            /// </summary>
            /// <param name="responseString">Serialized XML string.</param>
            /// <param name="response">Deserialized XML document.</param>
            /// <returns>The deserialization result.</returns>
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
