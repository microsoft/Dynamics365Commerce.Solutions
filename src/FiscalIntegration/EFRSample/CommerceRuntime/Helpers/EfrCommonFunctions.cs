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
    namespace CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;

        /// <summary>
        /// Contains helper methods for sales transaction document generation.
        /// </summary>
        internal static class EfrCommonFunctions
        {
            /// <summary>
            /// Converts line number to position number.
            /// </summary>
            /// <param name="lineNumber">The line number.</param>
            /// <returns>The position number.</returns>
            public static int ConvertLineNumberToPositionNumber(decimal lineNumber)
            {
                return Convert.ToInt32(lineNumber);
            }

            /// <summary>
            /// Gets the line number for deposit positions.
            /// </summary>
            /// <param name="positions">The collection of receipt positions.</param>
            /// <returns>The deposit position number.</returns>
            public static int GetDepositPositionNumber(IEnumerable<ReceiptPosition> positions)
            {
                return positions.DefaultIfEmpty(new ReceiptPosition() { PositionNumber = 0 })
                    .Max(c => c.PositionNumber) + 1;
            }

            /// <summary>
            /// Gets the joined tax codes string.
            /// </summary>
            /// <param name="taxCodes">The tax codes collection.</param>
            /// <returns>The tax codes string.</returns>
            public static string JoinTaxCodes(IEnumerable<string> taxCodes)
            {
                return string.Join(" ", taxCodes);
            }
        }
    }
}
