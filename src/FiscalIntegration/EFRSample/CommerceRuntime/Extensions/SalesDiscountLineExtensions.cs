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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Extensions
    {
        using System;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.DocumentHelpers;

        /// <summary>
        /// The sales discount line extensions.
        /// </summary>
        internal static class SalesDiscountLineExtensions
        {
            /// <summary>
            /// Gets discount name.
            /// </summary>
            /// <param name="discountLineType">The discount line.</param>
            /// <returns>The discount name.</returns>
            public static string DiscountName(this DiscountLine discountLine)
            {
                string discountName = string.Empty;

                if (discountLine.Percentage != 0)
                {
                    discountName = $"{FormatHelper.FormatDecimal(-Math.Sign(discountLine.EffectiveAmount) * discountLine.Percentage)}%";
                }

                return discountName;
            }
        }
    }
}
