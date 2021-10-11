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
        using CleanCash.Interop;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals.Entities;

        internal static class ErrorCodesMapper
        {
            public static FiscalPeripheralFailureType MapToFiscalPeripheralFailureType(CommunicationResult result)
            {
                switch (result)
                {
                    case CommunicationResult.RC_SUCCESS:
                        return FiscalPeripheralFailureType.None;
                    case CommunicationResult.RC_E_INVALID_PARAMETER:
                    case CommunicationResult.RC_E_NULL_PARAMETER:
                        return FiscalPeripheralFailureType.SubmissionFailed;
                    case CommunicationResult.RC_E_PORT_BUSY:
                        return FiscalPeripheralFailureType.Busy;
                    default:
                        return FiscalPeripheralFailureType.Other;
                }
            }
        }
    }
}