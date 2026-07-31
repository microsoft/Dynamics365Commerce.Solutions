/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample.Options
{
    using System.ComponentModel.DataAnnotations;

    public class RetailServerClientOptions
    {
        [Required()]
        public string TenantId { get; set; }

        [Required()]
        public string RetailServerAudienceId { get; set; }

        [Required()]
        public string AppIdKeyVaultSecretName { get; set; }

        [Required()]
        public string AppSecretKeyVaultSecretName { get; set; }

        [Required()]
        public string RetailServerUri { get; set; }

        [Required()]
        public string OperatingUnitNumber { get; set; }

        [Required()]
        public string ReturnToCartUrl { get; set; }

        [Required()]
        public long IncludeAbandonedCartsModifiedSinceLastMinutes { get; set; }

        [Required()]
        public long ExcludeAbandonedCartsModifiedSinceLastMinutes { get; set; }
    }

}
