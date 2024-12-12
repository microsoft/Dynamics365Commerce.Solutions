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
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Generates X report or Z report document.
        /// </summary>
        public static class XZReportDocumentBuilder
        {
            private const string DefaultTimeOutMilliseconds = "30";

            /// <summary>
            /// Builds X report or Z report document.
            /// </summary>
            /// <param name="eventTypeValue">The fiscal integration event type.</param>
            /// <returns>The generated fiscal document.</returns>
            public static string Build(FiscalIntegrationEventType eventType)
            {
                XDocument doc = new XDocument();
                XElement rootElement = new XElement(DocumentElementConstants.RootForXZReport);

                XElement displayText = new XElement(DocumentElementConstants.DisplayText);
                displayText.Add(new XAttribute(DocumentAttributeConstants.Operator, DocumentAttributeConstants.DefaultOperatorId));
                displayText.Add(new XAttribute(DocumentAttributeConstants.Data, DocumentAttributeConstants.DataValue));

                XElement printXZReport;

                switch (eventType)
                {
                    case FiscalIntegrationEventType.FiscalXReport:
                        printXZReport = new XElement(DocumentElementConstants.PrintXReport);
                        break;

                    case FiscalIntegrationEventType.FiscalZReport:
                        printXZReport = new XElement(DocumentElementConstants.PrintZReport);
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type '{0}' is not supported.", eventType));
                }

                printXZReport.Add(new XAttribute(DocumentAttributeConstants.Operator, DocumentAttributeConstants.DefaultOperatorId));
                printXZReport.Add(new XAttribute(DocumentAttributeConstants.TimeOut, DefaultTimeOutMilliseconds));

                rootElement.Add(displayText);
                rootElement.Add(printXZReport);
                doc.Add(rootElement);

                return doc.ToString();
            }
        }
    }
}
