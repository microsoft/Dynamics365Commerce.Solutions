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
        /// Contains constants of elements for fiscal integration document.
        /// </summary>
        /// <remark>
        /// This is defined by a third-party library, or by the tax authority, etc.
        /// </remark>
        public static class DocumentElementConstants
        {
            /// <summary>
            /// The beginFiscalReceipt element.
            /// </summary>
            internal const string BeginFiscalReceipt = "beginFiscalReceipt";

            /// <summary>
            /// The displayText elements.
            /// </summary>
            internal const string DisplayText = "displayText";

            /// <summary>
            /// The endFiscalReceipt element.
            /// </summary>
            internal const string EndFiscalReceipt = "endFiscalReceipt";

            /// <summary>
            /// The printBarCode element.
            /// </summary>
            internal const string printBarCode = "printBarCode";

            /// <summary>
            /// The printRecItem element.
            /// </summary>
            internal const string PrintRecItem = "printRecItem";

            /// <summary>
            /// The printRecItemAdjustment element.
            /// </summary>
            internal const string PrintRecItemAdjustment = "printRecItemAdjustment";

            /// <summary>
            /// The printRecMessage element.
            /// </summary>
            internal const string PrintRecMessage = "printRecMessage";

            /// <summary>
            /// The printRecRefund element.
            /// </summary>
            internal const string PrintRecRefund = "printRecRefund";

            /// <summary>
            /// The printRecSubtotalAdjustment element.
            /// </summary>
            internal const string PrintRecSubtotalAdjustment = "printRecSubtotalAdjustment";

            /// <summary>
            /// The printRecTotal element.
            /// </summary>
            internal const string PrintRecTotal = "printRecTotal";

            /// <summary>
            /// The printXReport element.
            /// </summary>
            internal const string PrintXReport = "printXReport";

            /// <summary>
            /// The printZReport element.
            /// </summary>
            internal const string PrintZReport = "printZReport";

            /// <summary>
            /// The XML root element for sales order.
            /// </summary>
            internal const string RootForSalesOrder = "printerFiscalReceipt";

            /// <summary>
            /// The XML root element for X and Z report.
            /// </summary>
            internal const string RootForXZReport = "printerFiscalReport";

            /// <summary>
            /// The directIO element;
            /// </summary>
            internal static string directIO = "directIO";

            /// <summary>
            /// The direct IO command to print the lottery code.
            /// </summary>
            internal static string lotteryCodeDirectIOCommand = "1135";
        }
    }
}
