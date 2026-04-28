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
        using System;
        using CleanCash.Interop;

        /// <summary>
        /// Thrown during the fiscal register errors.
        /// </summary>
        public class CleanCashDeviceException : Exception
        {
            /// <summary>
            /// The communication error code.
            /// </summary>
            public CommunicationResult ResultCode { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CleanCashDeviceException"/> class.
            /// </summary>
            /// <param name="resultCode">The communication error code.</param>
            /// <param name="errorMessage">The message containing format strings.</param>
            public CleanCashDeviceException(CommunicationResult resultCode, string errorMessage)
                : base(errorMessage)
            {
                this.ResultCode = resultCode;
            }
        }
    }
}
