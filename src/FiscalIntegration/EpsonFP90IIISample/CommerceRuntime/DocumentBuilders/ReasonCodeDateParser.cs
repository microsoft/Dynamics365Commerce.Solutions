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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.DocumentBuilders
    {
        using System;
        using System.Globalization;

        /// <summary>
        /// Parses the reason code dates.
        /// </summary>
        public static class ReasonCodeDateParser
        {
            private static string[] ExpectedItalianDateFormats = { "ddd d MMM yyyy", "dddd d MMMM yyyy", "d MMMM yyyy", "d MMM yyyy", "dd/MM/yyyy" };
            private const string LeftToRightMark = "\u200e";

            /// <summary>
            /// Parses the reason code return date text.
            /// </summary>
            /// <param name="date">The reason code return date string.</param>
            /// <param name="parsedDate">The converted datetime object is returned if the conversion succeeded; if failed, the default datetime object value is returned.</param>
            /// <returns>True if the date parameter was converted successfully; otherwise, false./</returns>
            public static bool TryParse(string date, out DateTime parsedDate)
            {
                // Unwanted Left-To-Right marks are added when returning a product in Modern POS.
                var adjustedReturnDate = date.Replace(LeftToRightMark, String.Empty);
                DateTime fiscalReceiptDate;
                var isReturnDateParsed = DateTime.TryParseExact(adjustedReturnDate, ExpectedItalianDateFormats, new CultureInfo("it-IT"), DateTimeStyles.None, out fiscalReceiptDate)
                    || DateTime.TryParse(adjustedReturnDate, out fiscalReceiptDate);
                parsedDate = fiscalReceiptDate;
                return isReturnDateParsed;
            }
        }
    }
}