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
    namespace HardwareStation.Connector.AtolSample.DataModel.Configuration
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents connector settings.
        /// </summary>
        [DataContract]
        public class ConnectorSettings
        {
            /// <summary>
            /// Gets or sets COM port value.
            /// </summary>
            [DataMember]
            public string ComPort { get; set; }

            /// <summary>
            /// Gets or sets baud rate value.
            /// </summary>
            [DataMember]
            public int BaudRate { get; set; }

            /// <summary>
            /// Gets or sets print previous printed document flag.
            /// </summary>
            [DataMember]
            public bool PrintPreviousNotPrintedDocument { get; set; }
        }
    }
}
