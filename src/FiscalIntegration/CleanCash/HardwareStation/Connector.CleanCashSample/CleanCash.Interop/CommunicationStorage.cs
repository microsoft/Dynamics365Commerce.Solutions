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
        /// List of storage status of the CleanCash unit.
        /// </summary>
        public enum CommunicationStorage
        {
            /// <summary>
            /// Unknown status.
            /// </summary>
            STORAGE_STATUS_UNKNOWN = -1,
            /// <summary>
            /// Status is OK.
            /// </summary>
            STORAGE_STATUS_OK,
            /// <summary>
            /// Internal storage is becoming full.
            /// </summary>
            STORAGE_STATUS_HIGH,
            /// <summary>
            /// Internal storage is full.
            /// </summary>
            STORAGE_STATUS_FULL
        }
    }
}