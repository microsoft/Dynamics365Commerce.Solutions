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
        /// The CleanCash unit types.
        /// </summary>
        public enum CommunicationUnitType
        {
            /// <summary>
            /// Unknown type.
            /// </summary>
            UNIT_TYPE_UNKNOWN = -1,
            /// <summary>
            /// CleanCash Type A.
            /// </summary>
            UNIT_TYPE_A,
            /// <summary>
            /// CleanCash MultiUser.
            /// </summary>
            UNIT_TYPE_MU = 2,
            /// <summary>
            /// CleanCash C1 (USB Single User).
            /// </summary>
            UNIT_TYPE_C1,
            /// <summary>
            /// CleanCash MultiUser C1/F (multiple companies, one POS ID).
            /// </summary>
            UNIT_TYPE_C1_F
        }
    }
}