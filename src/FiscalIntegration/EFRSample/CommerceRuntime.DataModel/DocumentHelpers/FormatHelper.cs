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
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.DocumentHelpers
    {
        using System;
        using System.Globalization;

        /// <summary>
        /// Contains helper methods for sales transaction document generation.
        /// </summary>
        public static class FormatHelper
        {
            /// <summary>
            /// Decimal format.
            /// </summary>
            public const string DecimalFormat = "F2";

            /// <summary>
            /// DateTime format.
            /// </summary>
            public const string DateTimeFormat = "s";

            /// <summary>
            /// Formats DateTime value.
            /// </summary>
            /// <param name="dateTimeValue">The DateTime value.</param>
            /// <returns>The formatted DateTime value.</returns>
            public static string FormatDateTime(DateTime dateTimeValue)
            {
                return dateTimeValue == DateTime.MinValue ? string.Empty : dateTimeValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Formats decimal value.
            /// </summary>
            /// <param name="decimalTimeValue">The decimal value.</param>
            /// <returns>The formatted decimal value.</returns>
            public static string FormatDecimal(decimal? decimalValue)
            {
                return decimalValue?.ToString(DecimalFormat, CultureInfo.InvariantCulture);
            }

        }
    }
}
