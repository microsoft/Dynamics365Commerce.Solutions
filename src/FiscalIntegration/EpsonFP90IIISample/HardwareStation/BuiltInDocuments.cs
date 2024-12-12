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
        using System;

        /// <summary>
        /// Built-in documents to execute technical requests to the fiscal printer.
        /// </summary>
        internal static class BuiltInDocuments
        {
            /// <summary>
            /// Gets document to set current date and time for printer.
            /// </summary>
            /// <returns>Returns documents to set date and time.</returns>
            internal static string GetSetCurrentDateTimeDocument()
            {
                DateTime currentDateTime = DateTime.Now;

                return string.Format(
                    @"<printerCommand><setDate day=""{0}"" month=""{1}"" year=""{2}"" hour=""{3}"" minute=""{4}"" /></printerCommand>",
                    currentDateTime.Day, currentDateTime.Month, currentDateTime.Year, currentDateTime.Hour, currentDateTime.Minute);
            }

            /// <summary>
            /// Gets check status document.
            /// </summary>
            /// <returns>The document to check status of the printer.</returns>
            internal static string GetCheckStatusDocument()
                => @"<printerCommand><queryPrinterStatus operator="""" statusType="""" /></printerCommand>";

            /// <summary>
            /// Gets document to read fiscal serial numhber from the printer.
            /// </summary>
            /// <returns>The document with information about fiscal printer serial number.</returns>
            internal static string GetFiscalSerialNumber()
                => @"<printerCommand><directIO command=""3217"" data=""01""/></printerCommand>";

            /// <summary>
            /// Gets document to read RT mode status of the printer.
            /// </summary>
            /// <returns>The document with information about fiscal printer RT mode status.</returns>
            internal static string GetRTModeStatus()
                => @"<printerCommand><directIO command=""1138"" data=""01""/></printerCommand>";
        }
    }
}
