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
        using System.Linq;
        using System.Xml.Linq;
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Validates the generated fiscal integration document.
        /// </summary>
        public static class DocumentValidator
        {
            /// <summary>
            /// Gets whether document generation is required for a sales order.
            /// </summary>
            /// <param name="salesOrder">The sales order.</param>
            /// <param name="eventType">The fiscal integration event type.</param>
            /// <returns>True if document generation is required, otherwise false.</returns>
            /// <remarks>The sales order should be adjusted.</remarks>
            public static bool IsDocumentGenerationRequired(SalesOrder salesOrder, FiscalIntegrationEventType eventType)
            {
                return eventType == FiscalIntegrationEventType.Sale && salesOrder.ActiveSalesLines.Any();
            }

            /// <summary>
            /// Validates fiscal integration document structure.
            /// </summary>
            /// <param name="document">The generated document string.</param>
            /// <param name="eventType">The fiscal integration event type.</param>
            /// <returns>The document generation result.</returns>
            public static FiscalIntegrationDocumentGenerationResultType ValidateDocumentStructure(string document, FiscalIntegrationEventType eventType)
            {
                if (string.IsNullOrWhiteSpace(document))
                {
                    return FiscalIntegrationDocumentGenerationResultType.None;
                }

                XElement doc = XElement.Parse(document);

                switch (eventType)
                {
                    case FiscalIntegrationEventType.Sale:
                        return ValidateFiscalReceiptDocument(doc);

                    case FiscalIntegrationEventType.FiscalXReport:
                    case FiscalIntegrationEventType.FiscalZReport:
                        return ValidateXZReportDocument(doc, eventType);

                    default:
                        throw new NotSupportedException(string.Format("Fiscal integration event type value '{0}' is not supported.", eventType));
                }
            }

            /// <summary>
            /// Validates fiscal receipt document structure.
            /// </summary>
            /// <param name="element">The generated document string.</param>
            /// <returns>The document generation result.</returns>
            private static FiscalIntegrationDocumentGenerationResultType ValidateFiscalReceiptDocument(XElement element)
            {
                FiscalIntegrationDocumentGenerationResultType resultType = FiscalIntegrationDocumentGenerationResultType.Failed;

                // Validates root element.
                if (element.Name == DocumentElementConstants.RootForSalesOrder)
                {
                    // Validates endFiscalReceipt element.
                    if (element.Descendants(DocumentElementConstants.EndFiscalReceipt).FirstOrDefault() != null)
                    {
                        // Validates printRecTotal element.
                        if (element.Descendants(DocumentElementConstants.PrintRecTotal).FirstOrDefault() != null)
                        {
                            // Validates beginFiscalReceipt, printRecItem and PrintRecRefund element.
                            if (element.Descendants(DocumentElementConstants.BeginFiscalReceipt).FirstOrDefault() != null
                                    && (element.Descendants(DocumentElementConstants.PrintRecItem).FirstOrDefault() != null
                                        || element.Descendants(DocumentElementConstants.PrintRecRefund).FirstOrDefault() != null))
                            {
                                resultType = FiscalIntegrationDocumentGenerationResultType.Succeeded;
                            }
                        }
                    }
                }

                return resultType;
            }

            /// <summary>
            /// Validates fiscal integration document structure.
            /// </summary>
            /// <param name="element">The generated document string.</param>
            /// <param name="eventType">The fiscal integration event type.</param>
            /// <returns>The document generation result.</returns>
            private static FiscalIntegrationDocumentGenerationResultType ValidateXZReportDocument(XElement element, FiscalIntegrationEventType eventType)
            {
                FiscalIntegrationDocumentGenerationResultType resultType = FiscalIntegrationDocumentGenerationResultType.Failed;

                // Validates root element.
                if (element.Name == DocumentElementConstants.RootForXZReport)
                {
                    switch (eventType)
                    {
                        // Validates printXReport element.
                        case FiscalIntegrationEventType.FiscalXReport:
                            if (element.Descendants(DocumentElementConstants.PrintXReport).FirstOrDefault() != null)
                            {
                                resultType = FiscalIntegrationDocumentGenerationResultType.Succeeded;
                            }
                            break;

                        // Validates printZReport element.
                        case FiscalIntegrationEventType.FiscalZReport:
                            if (element.Descendants(DocumentElementConstants.PrintZReport).FirstOrDefault() != null)
                            {
                                resultType = FiscalIntegrationDocumentGenerationResultType.Succeeded;
                            }
                            break;

                        default:
                            throw new NotSupportedException(string.Format("Fiscal integration event type value '{0}' is not supported.", eventType));
                    }
                }

                return resultType;
            }
        }
    }
}
