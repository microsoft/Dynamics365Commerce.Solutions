/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample.Messaging
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class EmailPayload
    {
        [JsonProperty("key_id")]
        public string KeyId { get; set; }

        [JsonProperty("returnToCartUrl")]
        public string ReturnToCartUrl { get; set; }

        [JsonProperty("contacts")]
        public List<Contact> Contacts { get; set; }
    }

    public class Contact
    {
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("returnToCartUrl")]
        public string ReturnToCartUrl { get; set; }

        [JsonProperty("orderDetails")]
        public List<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productImage")]
        public ProductImage productImage { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("unitPrice")]
        public string UnitPrice { get; set; }
    }

    public class ProductImage
    {
        [JsonProperty("altText")]
        public string altText { get; set; }

        [JsonProperty("viewPortImages")]
        public List<ViewPortImage> ViewPortImages { get; set; }
    }

    public class ViewPortImage
    {
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("media")]
        public string media { get; set; }

        [JsonProperty("useForDefaultImageTag")]
        public bool UseForDefaultImageTag { get; set; }
    }
}

