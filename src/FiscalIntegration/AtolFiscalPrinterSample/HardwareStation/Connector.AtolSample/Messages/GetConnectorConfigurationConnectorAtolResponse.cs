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
    namespace HardwareStation.Connector.AtolSample.Messages
    {
        using Contoso.HardwareStation.Connector.AtolSample.DataModel.Configuration;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The service response to get connector configuration.
        /// </summary>
        public class GetConnectorConfigurationConnectorAtolResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetConnectorConfigurationConnectorAtolResponse"/> class.
            /// </summary>
            /// <param name="profile"></param>
            public GetConnectorConfigurationConnectorAtolResponse(ConnectorSettings profile)
            {
                this.ConnectorSettings = profile;
            }

            /// <summary>
            /// Gets connector settings.
            /// </summary>
            public ConnectorSettings ConnectorSettings { get; private set; }
        }
    }
}
