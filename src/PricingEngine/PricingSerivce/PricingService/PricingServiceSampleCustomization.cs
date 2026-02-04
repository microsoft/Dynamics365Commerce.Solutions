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
    namespace Commerce.Runtime.Extensions.PricingServicesSample
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Contoso.CommerceRuntime.PricingEngine;
        using PE = Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;

        /// <summary>
        /// Sample customization of pricing service.
        /// </summary>
        public class PricingServiceSampleCustomization : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(CalculatePricesServiceRequest),
                        typeof(CalculateDiscountsServiceRequest),
                        typeof(GetIndependentPriceDiscountServiceRequest),
                    };
                }
            }

            /// <summary>
            /// Implements customized solutions for pricing services.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <returns>The response object.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                Response response;
                switch (request)
                {
                    case CalculatePricesServiceRequest calculatePricesServiceRequest:
                        response = await this.CalculatePricesAsync(calculatePricesServiceRequest).ConfigureAwait(false);
                        break;
                    case CalculateDiscountsServiceRequest calculateDiscountsServiceRequest:
                        response = await this.CalculateDiscountAsync(calculateDiscountsServiceRequest).ConfigureAwait(false);
                        break;
                    case GetIndependentPriceDiscountServiceRequest getIndependentPriceDiscountServiceRequest:
                        response = await this.CalculateIndependentPriceAndDiscountAsync(getIndependentPriceDiscountServiceRequest).ConfigureAwait(false);
                        break;
                    default:
                        throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
                }

                return response;
            }

            private async Task<GetPriceServiceResponse> CalculatePricesAsync(CalculatePricesServiceRequest request)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(request.RequestContext, "request.RequestContext");
                ThrowIf.Null(request.Transaction, "request.Transaction");

                var response = await request.RequestContext.Runtime.ExecuteNextAsync<GetPriceServiceResponse>(this, request, request.RequestContext, skipRequestTriggers: false).ConfigureAwait(false);

                // response.Transaction contains the transaction information.
                // Extensions are free to lookup sales information such as delivery mode, customer's name, sales lines, prices, etc.
                // Extensions can also update prices based on their needs for this request.
                // However, you should find another request/response pair for updating other fields like delivery mode.

                return response;
            }

            private async Task<GetPriceServiceResponse> CalculateDiscountAsync(CalculateDiscountsServiceRequest request)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(request.RequestContext, "request.RequestContext");
                ThrowIf.Null(request.Transaction, "request.Transaction");

                // Uncomment to register amount cap discount.
                // PE.IDiscountPackage package = new DiscountPackageAmountCap(new ChannelDataAccessorDiscountAmountCap(request.RequestContext));
                // PE.PricingEngineExtensionRepository.RegisterDiscountPackage(package);

                var response = await request.RequestContext.Runtime.ExecuteNextAsync<GetPriceServiceResponse>(this, request, request.RequestContext, skipRequestTriggers: false).ConfigureAwait(false);

                // response.Transaction contains the transaction information.
                // Extensions are free to lookup sales information such as delivery mode, customer's name, sales lines, prices, etc.
                // Extensions can also update prices based on their needs for this request.
                // However, you should find another request/response pair for updating other fields like delivery mode.

                return response;
            }

            private async Task<GetPriceServiceResponse> CalculateIndependentPriceAndDiscountAsync(
                GetIndependentPriceDiscountServiceRequest request)
            {
                ThrowIf.Null(request, nameof(request));
                ThrowIf.Null(request.RequestContext, "request.RequestContext");
                ThrowIf.Null(request.Transaction, "request.Transaction");

                var response = await request.RequestContext.Runtime.ExecuteNextAsync<GetPriceServiceResponse>(this, request, request.RequestContext, skipRequestTriggers: false).ConfigureAwait(false);

                // response.Transaction contains the transaction information.
                // Extensions are free to lookup sales information such as delivery mode, customer's name, sales lines, prices, etc.
                // Extensions can also update prices based on their needs for this request.
                // However, you should find another request/response pair for updating other fields like delivery mode.

                return response;
            }
        }
    }
}
