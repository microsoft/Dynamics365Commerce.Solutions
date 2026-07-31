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
    namespace HardwareStation.Connector.CleanCashSample.CleanCash.Interop
    {
        /// <summary>
        /// The statuses of the CleanCash unit.
        /// </summary>
        public enum CommunicationStatus
        {
            /// <summary>
            /// Status is OK.
            /// </summary>
            STATUS_OK,
            /// <summary>
            /// Warning (no fatal error).
            /// </summary>
            STATUS_WARNING,
            /// <summary>
            /// Protocol error (no fatal error).
            /// </summary>
            STATUS_PROTOCOL_ERROR,
            /// <summary>
            /// No fatal error.
            /// </summary>
            STATUS_ERROR,
            /// <summary>
            /// Fatal error.
            /// </summary>
            STATUS_FATAL_ERROR,
            /// <summary>
            /// Busy at the moment.
            /// </summary>
            STATUS_BUSY,
            /// <summary>
            /// Unknown status.
            /// </summary>
            STATUS_UNKNOWN,
            /// <summary>
            /// Communication error.
            /// </summary>
            STATUS_COMMUNICATION_ERROR = 99
        }
    }
}