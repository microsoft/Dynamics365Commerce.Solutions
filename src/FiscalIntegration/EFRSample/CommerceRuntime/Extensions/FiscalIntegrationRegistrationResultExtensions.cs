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
        using System.Linq;
        using System.Xml.Linq;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Serializers;
        using ReceiptsAustria = Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.Austria;
        using ReceiptsCzech = Microsoft.Dynamics.Commerce.Runtime.Localization.Entities.Czechia;

        /// <summary>
        /// The fiscal register response extensions.
        /// </summary>
        internal static class FiscalIntegrationRegistrationResultExtensions
        {
            private const string TLTAttributeName = "TLT";
            private const string SecAttributeName = "SEC";
            private const string SignAttributeName = "SIGN";
            private const string FiscalAttributeName = "FISCAL";
            private const string InfoAttributeName = "INFO";

            private const string TracElementName = "TraC";
            private const string ESRElementName = "ESR";
            private const string TransactionNumberAttributeName = "TN";
            private const string TransactionDateAttributeName = "D";
            private const string SequenceNumberAttributeName = "SQ";

            /// <summary>
            /// Gets the fiscal response string specific for Austria.
            /// </summary>
            /// <param name="fiscalRegistrationResult">The fiscal registration result.</param>
            /// <returns>The fiscal response string.</returns>
            public static string GetFiscalResponseStringAT(this FiscalIntegrationRegistrationResult fiscalRegistrationResult)
            {
                SalesTransactionRegistrationResponse salesTransactionResponse = XmlSerializer<SalesTransactionRegistrationResponse>.Deserialize(fiscalRegistrationResult.Response);

                var fiscalServiceResponse = new ReceiptsAustria.FiscalServiceResponse
                {
                    SequenceNumber = salesTransactionResponse.SequenceNumber,
                    FiscalQRCode = salesTransactionResponse.FiscalData?.FiscalQRCode,
                    SubmittedDocument = fiscalRegistrationResult.SubmittedDocument,
                    Response = fiscalRegistrationResult.Response,
                    InfoMessage = GetRecieptTagInfo(fiscalRegistrationResult.Response, InfoAttributeName)
                };

                return XmlSerializer<ReceiptsAustria.FiscalServiceResponse>.Serialize(fiscalServiceResponse);
            }

            /// <summary>
            /// Gets the fiscal response string specific for Czech Republic.
            /// </summary>
            /// <param name="fiscalRegistrationResult">The fiscal registration result.</param>
            /// <returns>The fiscal response string.</returns>
            public static string GetFiscalResponseStringCZ(this FiscalIntegrationRegistrationResult fiscalRegistrationResult)
            {
                var fiscalServiceResponse = new ReceiptsCzech.FiscalServiceResponse
                {
                    SubmittedDocument = fiscalRegistrationResult.SubmittedDocument,
                    Response = fiscalRegistrationResult.Response,
                    TransactionDate = GetSingleElementAttributeValue(fiscalRegistrationResult.Response, ESRElementName, TransactionDateAttributeName),
                    TransactionNumber = GetSingleElementAttributeValue(fiscalRegistrationResult.Response, ESRElementName, TransactionNumberAttributeName),
                    Tlt = GetRecieptTagInfo(fiscalRegistrationResult.Response, TLTAttributeName),
                    Sec = GetRecieptTagInfo(fiscalRegistrationResult.Response, SecAttributeName),
                    Sign = GetRecieptTagInfo(fiscalRegistrationResult.Response, SignAttributeName),
                    Fiscal = GetRecieptTagInfo(fiscalRegistrationResult.Response, FiscalAttributeName),
                    Info = GetRecieptTagInfo(fiscalRegistrationResult.Response, InfoAttributeName)
                };

                string sequenceNumberString = GetSingleElementAttributeValue(fiscalRegistrationResult.Response, TracElementName, SequenceNumberAttributeName);
                int sequenceNumber;
                if (Int32.TryParse(sequenceNumberString, out sequenceNumber))
                {
                    fiscalServiceResponse.SequenceNumber = sequenceNumber;
                }

                return XmlSerializer<ReceiptsCzech.FiscalServiceResponse>.Serialize(fiscalServiceResponse);
            }

            /// <summary>
            /// Deserializes XML string into generic container.
            /// </summary>
            /// <param name="responseString">The serialized XML string.</param>
            /// <param name="tagName">The name of a tag.</param>
            /// <returns>Container with deserialized XML document.</returns>
            private static string GetRecieptTagInfo(string responseString, string tagName)
            {
                XDocument doc = XDocument.Parse(System.Net.WebUtility.HtmlDecode(responseString));
                return doc.Descendants("Tag")
                    .Where(tag => string.Equals((string)tag.Attribute("Name"), tagName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(tag => (string)tag.Attribute("Value")).SingleOrDefault();
            }

            /// <summary>
            /// Gets the value of the specified xml attribute.
            /// </summary>
            /// <param name="responseString">The serialized XML string.</param>
            /// <param name="elementName">The xml element name.</param>
            /// <param name="attributeName">The xml attribute name.</param>
            /// <returns></returns>
            private static string GetSingleElementAttributeValue(string responseString, string elementName, string attributeName)
            {
                XDocument doc = XDocument.Parse(System.Net.WebUtility.HtmlDecode(responseString));
                return doc.Descendants(elementName).Attributes()
                    .SingleOrDefault(c => string.Equals(c.Name.LocalName, attributeName, StringComparison.InvariantCultureIgnoreCase))?.Value;
            }
        }
    }
}
