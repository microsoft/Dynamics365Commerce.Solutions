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
    namespace HardwareStation.Connector.EpsonFP90IIISample
    {
        /// <summary>
        /// Represent printer configuration model.
        /// </summary>
        internal class ConfigurationModel
        {
            /// <summary>
            /// Gets or sets printers endpoint address.
            /// </summary>
            [ConfigurationAttribute(ConfigurationNamespaceType.Connection, "EndPointAddress")]
            public string EndPointAddress { get; set; }

            /// <summary>
            /// Gets or sets the flag indicates that date and time synchronization required.
            /// </summary>
            [ConfigurationAttribute(ConfigurationNamespaceType.Settings, "DateTimeSynchronizationRequired")]
            public bool SynchronizeDateTime { get; set; }
        }
    }
}
