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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask
    {
        /// <summary>
        /// Represents the list of the tasks of the printer.
        /// </summary>
        public static class AtolTaskTypes
        {
            /// <summary>
            /// Gets task name to generate X-report.
            /// </summary>
            public static string XReport { get { return "reportX"; } }

            /// <summary>
            /// Gets task name to close shift on the printer.
            /// </summary>
            public static string CloseShift { get { return "closeShift"; } }

            /// <summary>
            /// Gets task name to register sale.
            /// </summary>
            public static string Sale { get { return "sale"; } }

            /// <summary>
            /// Gets task name to register sale return.
            /// </summary>
            public static string SaleReturn { get { return "saleReturn"; } }
        }
    }
}
