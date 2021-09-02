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
    namespace HardwareStation.Connector.EpsonFP90IIISample
    {
        using System;
        using System.Net.Http;
        using System.Text;
        using System.Threading.Tasks;
        using System.Xml.Linq;
        using Microsoft.Dynamics.Commerce.HardwareStation;

        /// <summary>
        /// Class implements methods of communication with printer.
        /// </summary>
        internal class PrinterCommunicationController
        {
            /// <summary>
            /// Submits document to the printer.
            /// </summary>
            /// <param name="document">The document to submit.</param>
            /// <param name="configuration">The printers configuration.</param>
            /// <returns>The response from printer.</returns>
            internal async Task<string> SubmitDocumentAsync(string document, ConfigurationModel configuration)
            {
                ThrowIf.Null(configuration, nameof(configuration));
                ThrowIf.NullOrWhiteSpace(configuration.EndPointAddress, nameof(configuration.EndPointAddress));

                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(configuration.EndPointAddress),
                        Method = HttpMethod.Post
                    };

                    XDocument xmlDocument = this.WrapDocumentInEnvelope(document);
                    request.Content = new StringContent(xmlDocument.ToString(), Encoding.UTF8, "text/xml");

                    using (HttpResponseMessage response = await client.PostAsync(request.RequestUri, request.Content).ConfigureAwait(false))
                    {
                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }
            }

            /// <summary>
            /// Wraps document to submit soap request to printer.
            /// </summary>
            /// <param name="document">The document to wrap.</param>
            /// <returns>The xnl document content.</returns>
            internal XDocument WrapDocumentInEnvelope(string document)
            {
                XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                XDocument documentXML = new XDocument(
                   new XDeclaration("1.0", "utf-8", string.Empty),
                   new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Body", XElement.Parse(document))));

                return documentXML;
            }
        }
    }
}