namespace Contoso
{
    namespace Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample
    {
        using System;
        using System.Globalization;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel;
        using Contoso.Commerce.HardwareStation.PosnetThermalFVFiscalPrinterSample.DocumentModel.Protocol;

        /// <summary>
        /// Provides the way to parse Posnet response data types.
        /// </summary>
        internal static class PosnetParser
        {
            private const string PosnetDateFormat = "yyyy-MM-dd";
            private const string PosnetDateTimeFormat = "yyyy-MM-dd;HH:mm";
            private static readonly string[] PosnetBoolTrueValues = new string[] { "T", "Y", "1" };

            /// <summary>
            /// Parses the Num Posnet type.
            /// </summary>
            /// <param name="value">The value to be parsed.</param>
            /// <returns>Parsed value.</returns>
            internal static CommandParameterValue ParseNum(string value)
            {
                decimal valueDecimal = decimal.Parse(value);
                return new CommandParameterValue(valueDecimal, DataType.Num);
            }

            /// <summary>
            /// Parses the Amount Posnet type.
            /// </summary>
            /// <param name="value">The value to be parsed.</param>
            /// <returns>Parsed value.</returns>
            internal static CommandParameterValue ParseAmount(string value)
            {
                const decimal amountDivider = 100;

                decimal valueDecimal = decimal.Parse(value);
                return new CommandParameterValue(valueDecimal / amountDivider, DataType.Amount);
            }

            /// <summary>
            /// Parses the Alphanum Posnet type.
            /// </summary>
            /// <param name="value">The value to be parsed.</param>
            /// <returns>Parsed value.</returns>
            internal static CommandParameterValue ParseAlphanum(string value)
            {
                return new CommandParameterValue(value, DataType.Alphanum);
            }

            /// <summary>
            /// Parses the Date Posnet type.
            /// </summary>
            /// <param name="value">The value to be parsed.</param>
            /// <returns>Parsed value.</returns>
            internal static CommandParameterValue ParseDate(string value)
            {
                DateTime valueDateTime = DateTime.ParseExact(value, PosnetDateFormat, null, DateTimeStyles.None);
                return new CommandParameterValue(valueDateTime, DataType.Date);
            }

            /// <summary>
            /// Parses the DateTime Posnet type.
            /// </summary>
            /// <param name="value">The value to be parsed.</param>
            /// <returns>Parsed value.</returns>
            internal static CommandParameterValue ParseDateTime(string value)
            {
                DateTime valueDateTime = DateTime.ParseExact(value, PosnetDateTimeFormat, null, DateTimeStyles.None);
                return new CommandParameterValue(valueDateTime, DataType.DateTime);
            }

            /// <summary>
            /// Parses the Boolean Posnet type.
            /// </summary>
            /// <param name="value">The value to be parsed.</param>
            /// <returns>Parsed value.</returns>
            internal static CommandParameterValue ParseBoolean(string value)
            {
                bool valueBoolean = Array.Exists(PosnetBoolTrueValues, trueValue => trueValue.Equals(value, StringComparison.OrdinalIgnoreCase));
                return new CommandParameterValue(valueBoolean, DataType.Boolean);
            }
        }
    }
}