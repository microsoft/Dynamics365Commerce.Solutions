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
    namespace CommerceRuntime.Connector.SequentialSignatureNorwaySample.Configuration
    {
        using System.Linq;
        using System.Xml.Linq;
        using Microsoft.Dynamics.Commerce.Runtime;

        /// <summary>
        /// Reads the settings from service connector configuration.
        /// </summary>
        internal static class ConfigurationController
        {
            /// <summary>
            /// Gets the hash algorithm name.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The hash algorithm name.</returns>
            internal static string GetHashAlgorithmName(string configurationString)
            {
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.HashAlgorithmName,
                    ConfigurationElementConstants.StringValueElement);

                if (string.IsNullOrEmpty(xmlElement?.Value))
                {
                    ThrowValueRequiredException(xmlElement?.Value);
                }

                return xmlElement.Value.ToUpperInvariant();
            }

            /// <summary>
            /// Gets the secret name.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The secret name.</returns>
            internal static string GetSecretName(string configurationString)
            {
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.SecretName,
                    ConfigurationElementConstants.StringValueElement);

                return xmlElement?.Value;
            }

            /// <summary>
            /// Gets the local certificate thumbprint.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The local certificate thumbprint.</returns>
            internal static string GetLocalCertificateThumbprint(string configurationString)
            {
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.LocalCertificateThumbprint,
                    ConfigurationElementConstants.StringValueElement);

                return xmlElement?.Value;
            }

            /// <summary>
            /// Gets the certificate store name.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The certificate store name.</returns>
            internal static string GetCertificateStoreName(string configurationString)
            {
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.CertificateStoreName,
                    ConfigurationElementConstants.StringValueElement);

                return xmlElement?.Value;
            }

            /// <summary>
            /// Gets the certificate store location.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>The certificate store location.</returns>
            internal static string GetCertificateStoreLocation(string configurationString)
            {
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.CertificateStoreLocation,
                    ConfigurationElementConstants.StringValueElement);

                return xmlElement?.Value;
            }

            /// <summary>
            /// Gets the value indicating whether or not to obtain the certificate from the local certificate storage. Otherwise, the certificate will be obtained from the Key Vault.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>True if it is required to obtain local certificate firstly otherwise false.</returns>
            /// <remarks>If <c>xmlElement</c> is null or empty then return false to not to block registration.</remarks>
            internal static bool GetTryLocalCertificateFirst(string configurationString)
            {
                var isLocalFirst = false;
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.TryLocalCertificateFirst,
                    ConfigurationElementConstants.BooleanValueElement);
                if (!string.IsNullOrEmpty(xmlElement?.Value))
                {
                    bool.TryParse(xmlElement.Value, out isLocalFirst);
                }

                return isLocalFirst;
            }

            /// <summary>
            /// Gets the value indicating whether or not it is required to send the 'IsReady' request.
            /// </summary>
            /// <param name="configurationString">The configuration string.</param>
            /// <returns>Is it required to send the 'IsReady' request.</returns>
            /// <remarks>If <c>xmlElement</c> is null or empty then return false to not to block registration.</remarks>
            internal static bool GetIsReadyRequired(string configurationString)
            {
                var isReadyRequired = false;
                XElement xmlElement = GetElement(
                    configurationString,
                    ConfigurationElementConstants.ConnectorSettingsInfo,
                    ConfigurationElementConstants.IsReadyRequired,
                    ConfigurationElementConstants.BooleanValueElement);
                if (!string.IsNullOrEmpty(xmlElement?.Value))
                {
                    bool.TryParse(xmlElement.Value, out isReadyRequired);
                }

                return isReadyRequired;
            }

            /// <summary>
            /// Find the element for specific name and namespace of the fiscal service configuration.
            /// </summary>
            /// <param name="document">The fiscal document provider XML configuration.</param>
            /// <param name="namespaceValue">The namespace.</param>
            /// <param name="nameValue">The element name.</param>
            /// <param name="typeValue">The element value type.</param>
            /// <returns>The XML element.</returns>
            private static XElement GetElement(string document, string namespaceValue, string nameValue, string typeValue)
            {
                XElement configurationElement = XElement.Parse(document)
                    .Descendants(ConfigurationElementConstants.PropertyElement)
                    .SingleOrDefault(a =>
                        a.Element(ConfigurationElementConstants.NamespaceElement).Value == namespaceValue
                        && a.Element(ConfigurationElementConstants.NameElement).Value == nameValue);
                return configurationElement?.Descendants(typeValue).FirstOrDefault();
            }

            /// <summary>
            /// Throws DataValidationException if a required field is empty.
            /// </summary>
            /// <param name="requiredField">The required field.</param>
            private static void ThrowValueRequiredException(string requiredField)
            {
                throw new DataValidationException(
                    DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_RequiredValueNotFound,
                    string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} is empty", nameof(requiredField)));
            }
        }
    }
}