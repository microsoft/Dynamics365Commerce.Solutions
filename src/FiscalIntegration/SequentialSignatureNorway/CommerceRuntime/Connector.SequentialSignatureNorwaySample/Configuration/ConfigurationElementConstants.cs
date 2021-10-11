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
        /// <summary>
        /// The elements for fiscal service connector configuration.
        /// </summary>
        internal static class ConfigurationElementConstants
        {
            /// <summary>
            /// The BooleanValue element.
            /// </summary>
            internal const string BooleanValueElement = "BooleanValue";

            /// <summary>
            /// The inner text of Name element for SecretName property.
            /// </summary>
            internal const string SecretName = "SecretName";

            /// <summary>
            /// The inner text of Name element for LocalCertificateThumbprint property.
            /// </summary>
            internal const string LocalCertificateThumbprint = "LocalCertificateThumbprint";

            /// <summary>
            /// The inner text of Namespace element for ConnectorSettingsInfo.
            /// </summary>
            internal const string ConnectorSettingsInfo = "ConnectorSettingsInfo";

            /// <summary>
            /// The inner text of Name element for HashAlgorithmName property.
            /// </summary>
            internal const string HashAlgorithmName = "HashAlgorithmName";

            /// <summary>
            /// The inner text of Name element for CertificateStoreName property.
            /// </summary>
            internal const string CertificateStoreName = "CertificateStoreName";

            /// <summary>
            /// The inner text of Name element for CertificateStoreLocation property.
            /// </summary>
            internal const string CertificateStoreLocation = "CertificateStoreLocation";

            /// <summary>
            /// The inner text of Name element for TryLocalCertificateFirst property.
            /// </summary>
            internal const string TryLocalCertificateFirst = "TryLocalCertificateFirst";

            /// <summary>
            /// The inner text of Name element for IsReadyRequired property.
            /// </summary>
            internal const string IsReadyRequired = "IsReadyRequired";

            /// <summary>
            /// The name of the Name element.
            /// </summary>
            internal const string NameElement = "Name";

            /// <summary>
            /// The name of the Namespace element.
            /// </summary>
            internal const string NamespaceElement = "Namespace";

            /// <summary>
            /// The name of the Property element.
            /// </summary>
            internal const string PropertyElement = "ConfigurationProperty";

            /// <summary>
            /// The StringValue element.
            /// </summary>
            internal const string StringValueElement = "StringValue";
        }
    }
}