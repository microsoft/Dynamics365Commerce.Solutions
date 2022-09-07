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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Serializers
    {
        using System.IO;
        using System.Xml;
        using System.Xml.Serialization;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;

        /// <summary>
        /// Serializes the sales transaction document.
        /// </summary>
        internal static class FiscalDocumentSerializer
        {
            /// <summary>
            /// Gets the XML string representation of a fiscal document without the XML declaration node.
            /// </summary>
            /// <param name="document">The fiscal document.</param>
            /// <returns>The serialized string.</returns>
            public static string Serialize(IFiscalIntegrationDocument document)
            {
                if (document == null)
                {
                    return string.Empty;
                }

                XmlSerializer xmlSerializer = new XmlSerializer(document.GetType());

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(prefix: "", ns: "");

                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                    {
                        xmlSerializer.Serialize(xmlWriter, document, ns);

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(stringWriter.ToString());

                        if (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            xmlDocument.RemoveChild(xmlDocument.FirstChild);
                        }

                        return xmlDocument.InnerXml;
                    }
                }
            }
        }
    }
}
