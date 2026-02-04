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
        /// The result codes returned by CleanCash unit.
        /// </summary>
        public enum CommunicationResult
        {
            /// <summary>
            /// Succeeded.
            /// </summary>
            RC_SUCCESS,
            /// <summary>
            /// The call Illegal from current state.
            /// </summary>
            RC_E_ILLEGAL,
            /// <summary>
            /// Failed.
            /// </summary>
            RC_E_FAILURE,
            /// <summary>
            /// The port/link requested is busy.
            /// </summary>
            RC_E_PORT_BUSY,
            /// <summary>
            /// The port/link requested does not exist.
            /// </summary>
            RC_E_INVALID_PORT,
            /// <summary>
            /// One or more null parameter(s) has been sent.
            /// </summary>
            RC_E_NULL_PARAMETER,
            /// <summary>
            /// One or more parameter(s) has an invalid value.
            /// </summary>
            RC_E_INVALID_PARAMETER,
            /// <summary>
            /// Licenses exceeded.
            /// </summary>
            RC_E_LICENSE_EXCEEDED,
            RC_E_NOT_CLEANCASH,
            RC_E_NOT_SUPPORTED
        }
    }
}