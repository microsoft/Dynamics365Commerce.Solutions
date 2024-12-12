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
    namespace CommerceRuntime.DocumentProvider.SequentialSignNorway
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using Microsoft.Dynamics.Commerce.Runtime.Framework.Serialization;

        /// <summary>
        /// Encapsulates formatting operations required to prepare data for fiscal registration.
        /// </summary>
        internal class SequentialSignatureDataFormatter
        {
            private const string TimeFormat = "hh:mm:ss";
            private const string DateFormat = "yyyy-MM-dd";
            private const string FieldDelimiter = ";";

            /// <summary>
            /// Gets the formatted data to register string for a specified collection of separate fields.
            /// </summary>
            /// <param name="dataToRegisterFields">Separate fields of data to register string.</param>
            /// <returns>The formatted string.</returns>
            internal static string GetFormattedDataToRegister(IEnumerable<string> dataToRegisterFields) =>
                string.Join(FieldDelimiter, dataToRegisterFields);

            /// <summary>
            /// Formats a date.
            /// </summary>
            /// <param name="date">The date to format.</param>
            /// <returns>The formatted string.</returns>
            internal static string GetFormattedDate(DateTimeOffset date) => date.ToString(DateFormat);

            /// <summary>
            /// Formats a time.
            /// </summary>
            /// <param name="time">The time to format.</param>
            /// <returns>The formatted string.</returns>
            internal static string GetFormattedTime(DateTimeOffset time) => time.ToString(TimeFormat);

            /// <summary>
            /// Formats an amount.
            /// </summary>
            /// <param name="amount">The amount to format.</param>
            /// <returns>The formatted string.</returns>
            internal static string GetFormattedAmount(decimal amount) => amount.ToString("0.00", CultureInfo.InvariantCulture);

            /// <summary>
            /// Formats a sequential number.
            /// </summary>
            /// <param name="sequentialNumber">The sequential number to format.</param>
            /// <returns>The formatted string.</returns>
            internal static string GetFormattedSequentialNumber(long sequentialNumber) => sequentialNumber.ToString();

            /// <summary>
            /// Gets the last signature from previous registration response.
            /// </summary>
            /// <param name="response">The previous registered response.</param>
            /// <returns>The sequential signature string.</returns>
            internal static string GetLastSignatureFromResponse(string response)
            {
                JsonHelper.TryDeserialize<FiscalRegisterResult>(response, out var signatureData);

                return signatureData?.Signature;
            }
        }
    }
}
