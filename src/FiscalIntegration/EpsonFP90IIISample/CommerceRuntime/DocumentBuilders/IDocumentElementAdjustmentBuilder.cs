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
        using Contoso.CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using System.Xml.Linq;

        public interface IDocumentElementAdjustmentBuilder
        {
            /// <summary>
            /// Builds PrintRecItemAdjustment element or PrintSubtotalAdjustment element. If current line is sales line, builds PrintRecItemAdjustment element, else builds PrintSubtotalAdjustment line.
            /// The PrintRecItemAdjustment element is used for the discount amount of a sales line, the PrintSubtotalAdjustment is used for the total discount amount of a sales order.
            /// </summary>
            /// <param name="salesLine">The salesLine of current salesOrder.</param>
            /// <param name="rootElement">The root element for the fiscal receipt document's body.</param>
            /// <param name="description">Discounts description.</param>
            /// <param name="department">The department number value.</param>
            /// <param name="isSalesLine">A bool value indicates current record is salesLine or salesOrder.</param>
            /// <param name="operatorId">The value of the operatorId attribute.</param>
            /// <param name="justification">The value of the justification attribute.</param>
            /// <returns>The PrintRecItemAdjustment element or PrintSubtotalAdjustment element.</returns>
            XElement BuildPrintRecItemOrSubtotalAdjustment(
                SalesLine salesLine,
                XElement rootElement,
                string description,
                string department,
                bool isSalesLine = true,
                string operatorId = DocumentAttributeConstants.DefaultOperatorId,
                string justification = DocumentAttributeConstants.DefaultJustification);
        }
    }
}
