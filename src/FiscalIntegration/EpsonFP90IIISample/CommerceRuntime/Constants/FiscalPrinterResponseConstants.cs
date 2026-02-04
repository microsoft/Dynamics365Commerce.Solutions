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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants
    {
        /// <summary>
        /// Contains constants of elements for fiscal printer response.
        /// </summary>
        /// <remark>
        /// This is defined by a third-party library, or by the tax authority, etc.
        /// </remark>
        public static class FiscalPrinterResponseConstants
        {
            /// <summary>
            /// The addInfo element.
            /// </summary>
            internal const string AddInfoElement = "addInfo";

            /// <summary>
            /// The date format for fiscal documment.
            /// </summary>
            internal const string DateFormatForFiscalDocument = "ddMMyyyy";

            /// <summary>
            /// The date format in register response.
            /// </summary>
            internal const string DateFormatInRegisterResponse = "d/M/yyyy";

            /// <summary>
            /// The default value for fiscal receipt number.
            /// </summary>
            internal const int DefaultFiscalReceiptNumber = 9999;

            /// <summary>
            /// The default value for fiscal serial number.
            /// </summary>
            internal const string DefaultFiscalSerialNumber = "00000000000";

            /// <summary>
            /// The default value for Z report number.
            /// </summary>
            internal const int DefaultZRepNumber = 9999;

            /// <summary>
            /// The elementList element.
            /// </summary>
            internal const string ElementListElement = "elementList";

            /// <summary>
            /// The fiscalReceiptDate element.
            /// </summary>
            internal const string FiscalReceiptDateElement = "fiscalReceiptDate";

            /// <summary>
            /// The fiscalReceiptNumber element.
            /// </summary>
            internal const string FiscalReceiptNumberElement = "fiscalReceiptNumber";

            /// <summary>
            /// The number format for fiscal receipt number and Z report number.
            /// </summary>
            internal const string NumberFormat = "0000";

            /// <summary>
            /// The zRepNumber element.
            /// </summary>
            internal const string ZRepNumberElement = "zRepNumber";

            /// <summary>
            /// The attribute success in fiscal printer response file.
            /// </summary>
            internal const string Success = "success";
        }
    }
}
