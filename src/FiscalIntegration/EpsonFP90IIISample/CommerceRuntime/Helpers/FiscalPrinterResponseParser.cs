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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.Linq;
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Microsoft.Dynamics.Commerce.Runtime;

        /// <summary>
        /// The hepler class to parse fiscal printer response information.
        /// </summary>
        public class FiscalPrinterResponseParser
        {
            /// <summary>
            /// Gets the register response value by name.
            /// </summary>
            /// <param name="registerResponse">The register response XElement object.</param>
            /// <param name="elementName">The element name.</param>
            /// <returns>The value of specific element.</returns>
            public static string ParseRegisterResponseInfo(XElement registerResponse, string elementName)
            {
                ThrowIf.Null(registerResponse, nameof(registerResponse));
                ThrowIf.NullOrWhiteSpace(elementName, nameof(elementName));

                string elementList = GetElementValue(registerResponse, FiscalPrinterResponseConstants.ElementListElement);
                IEnumerable<string> splitElementsbyComma = elementList.Split(new[] { ',' });

                string elementValue = string.Empty;
                if (splitElementsbyComma.Contains(elementName))
                {
                    elementValue = GetElementValue(registerResponse, elementName);

                    // Converts date format from '(d)d/(M)M/YYYY' to "ddMMyyyy".
                    if (elementName == FiscalPrinterResponseConstants.FiscalReceiptDateElement)
                    {
                        elementValue = DateTime.ParseExact(elementValue, FiscalPrinterResponseConstants.DateFormatInRegisterResponse, CultureInfo.InvariantCulture).ToString(FiscalPrinterResponseConstants.DateFormatForFiscalDocument);
                    }
                }

                return elementValue;
            }

            /// <summary>
            /// Gets element value.
            /// </summary>
            /// <param name="element">The xml element.</param>
            /// <param name="elementName">The element name to find.</param>
            /// <returns>The element value.</returns>
            private static string GetElementValue(XElement element, string elementName)
            {
                var elementFound = element.Descendants(FiscalPrinterResponseConstants.AddInfoElement).Elements(elementName).SingleOrDefault();
                return elementFound?.Value ?? string.Empty;
            }

        }
    }
}
