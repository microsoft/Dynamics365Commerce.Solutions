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
    namespace HardwareStation.Connector.CleanCashSample
    {
        /// <summary>
        /// Clean cash fiscal register response.
        /// </summary>
        internal class CleanCashResponse
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CleanCashResponse" /> class.
            /// </summary>
            /// <param name="deviceId">Device ID.</param>
            /// <param name="controlCode">The control code.</param>
            public CleanCashResponse(string deviceId, string controlCode)
            {
                this.DeviceId = deviceId;
                this.ControlCode = controlCode;
            }

            /// <summary>
            /// Gets or sets the fiscal register device Id.
            /// </summary>
            public string DeviceId { get; set; }

            /// <summary>
            /// Gets or sets the fiscal data registration control code.
            /// </summary>
            public string ControlCode { get; set; }
        }
    }
}