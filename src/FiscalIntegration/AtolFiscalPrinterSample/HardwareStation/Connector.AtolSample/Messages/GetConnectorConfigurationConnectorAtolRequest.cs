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
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The service request to get connector configuration.
        /// </summary>
        public class GetConnectorConfigurationConnectorAtolRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetConnectorConfigurationConnectorAtolRequest"/> class.
            /// </summary>
            /// <param name="profile">Fiscal peripheral profile info.</param>
            public GetConnectorConfigurationConnectorAtolRequest(FiscalPeripheralInfo profile)
            {
                this.ConfigurationProperties = profile;
            }

            /// <summary>
            /// Gets fiscal peripheral properties.
            /// </summary>
            public FiscalPeripheralInfo ConfigurationProperties { get; private set; }
        }
    }
}
