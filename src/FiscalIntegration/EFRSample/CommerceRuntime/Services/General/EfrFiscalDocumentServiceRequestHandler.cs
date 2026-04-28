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
    namespace CommerceRuntime.DocumentProvider.EFRSample.Services.General
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
        using Contoso.CommerceRuntime.DocumentProvider.DataModelEFR.Documents;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.DocumentHelpers;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Extensions;
        using Contoso.CommerceRuntime.DocumentProvider.EFRSample.Messages;

        public sealed class EfrFiscalDocumentServiceRequestHandler : IRequestHandlerAsync
        {
            private const int RoundPrecision = 2;

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetEfrOperatorNameRequest),
                        typeof(GetEfrDepositOrderSumRequest),
                        typeof(GetEfrGetTenderTypeNameRequest),
                        typeof(PopulateEfrReferenceFieldsRequest),
                        typeof(PopulateEfrLocalizationInfoRequest),
                        typeof(PopulateEfrCustomerDataRequest),
                        typeof(PopulateCountryRegionSpecificPositionsRequest),
                        typeof(PopulateEfrLocalizationInfoToPaymentRequest),
                    };
                }
            }

            public async Task<Response> Execute(Request request)
            {
                switch (request)
                {
                    case GetEfrOperatorNameRequest getEfrOperatorNameRequest:
                        return await GetEfrOperatorName(getEfrOperatorNameRequest).ConfigureAwait(false);
                    case GetEfrDepositOrderSumRequest getDepositOrderSumRequest:
                        return await GetDepositOrderSum(getDepositOrderSumRequest).ConfigureAwait(false);
                    case GetEfrGetTenderTypeNameRequest getEfrGetTenderTypeNameRequest:
                        return await GetEfrGetTenderTypeName(getEfrGetTenderTypeNameRequest).ConfigureAwait(false);
                    case PopulateEfrReferenceFieldsRequest populateReferenceFieldsRequest:
                        return await PopulateReferenceFields(populateReferenceFieldsRequest).ConfigureAwait(false);
                    case PopulateEfrLocalizationInfoRequest gopulateEfrLocalizationInfoRequest:
                        return await PopulateEfrLocalizationInfo(gopulateEfrLocalizationInfoRequest).ConfigureAwait(false);
                    case PopulateEfrCustomerDataRequest populateEfrCustomerDataRequest:
                        return await PopulateEfrCustomerData(populateEfrCustomerDataRequest).ConfigureAwait(false);
                    case PopulateCountryRegionSpecificPositionsRequest populateCountryRegionSpecificPositionsRequest:
                        return await PopulateCountryRegionSpecificPositions(populateCountryRegionSpecificPositionsRequest).ConfigureAwait(false);
                    case PopulateEfrLocalizationInfoToPaymentRequest populateEfrLocalizationInfoToPaymentRequest:
                        return await PopulateEfrLocalizationInfoToPayment(populateEfrLocalizationInfoToPaymentRequest).ConfigureAwait(false);
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }
            }

            private static async Task<Response> GetEfrOperatorName(GetEfrOperatorNameRequest request)
            {
                GetEmployeesServiceRequest getEmployeeRequest = new GetEmployeesServiceRequest(request.OperatorId, QueryResultSettings.SingleRecord);
                GetEmployeesServiceResponse employeeResponse = await request.RequestContext.ExecuteAsync<GetEmployeesServiceResponse>(getEmployeeRequest).ConfigureAwait(false);

                Employee employee = employeeResponse.Employees.SingleOrDefault();

                var operatorName = employee?.Name ?? string.Empty;

                return new SingleEntityDataServiceResponse<string>(operatorName);
            }

            private static Task<Response> GetDepositOrderSum(GetEfrDepositOrderSumRequest request)
            {
                var activeTenderLinesAmount = request.SalesOrder.ActiveTenderLines.Sum(c => c.Amount);

                var carryoutLinesTotalAmount = request.SalesOrder
                    .GetCarryOutLines(request.RequestContext)
                    .Sum(c => c.TotalAmount);

                var depositSum = activeTenderLinesAmount - carryoutLinesTotalAmount;

                var amount = Math.Round(depositSum, RoundPrecision);

                Response response = new SingleEntityDataServiceResponse<decimal>(amount);
                return Task.FromResult(response);
            }

            private static async Task<Response> GetEfrGetTenderTypeName(GetEfrGetTenderTypeNameRequest request)
            {
                var requestContext = request.RequestContext;
                GetChannelTenderTypesDataRequest getChannelTenderTypesDataRequest = new GetChannelTenderTypesDataRequest(
                    requestContext.GetPrincipal().ChannelId,
                    QueryResultSettings.AllRecords);

                var tenderTypesResponse = await requestContext.Runtime.ExecuteAsync<EntityDataServiceResponse<TenderType>>(
                        getChannelTenderTypesDataRequest,
                        requestContext).ConfigureAwait(false);

                IReadOnlyCollection<TenderType> tenderTypes = tenderTypesResponse.PagedEntityCollection.Results;

                TenderType tenderType = tenderTypes.Single(t => t.TenderTypeId.Equals(request.TenderTypeId));

                return new SingleEntityDataServiceResponse<string>(tenderType.Name);
            }

            private static async Task<Response> PopulateReferenceFields(PopulateEfrReferenceFieldsRequest request)
            {
                var salesLine = request.SalesLine;
                var receiptPosition = request.ReceiptPosition;

                var originSalesOrderResponse = await salesLine.GetOriginSalesOrderAsync(request.RequestContext).ConfigureAwait(false);

                receiptPosition.ReferenceDateTime = originSalesOrderResponse?.CreatedDateTime.DateTime ?? DateTime.MinValue;
                receiptPosition.ReferenceTransactionLocation = salesLine.ReturnStore;
                receiptPosition.ReferenceTransactionTerminal = salesLine.ReturnTerminalId;
                receiptPosition.ReferenceTransactionNumber = salesLine.ReturnTransactionId;
                receiptPosition.ReferencePositionNumber = EfrCommonFunctions.ConvertLineNumberToPositionNumber(salesLine.ReturnLineNumber);

                return new SingleEntityDataServiceResponse<ReceiptPosition>(receiptPosition);
            }

            private static Task<Response> PopulateEfrLocalizationInfo(PopulateEfrLocalizationInfoRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt>(request.Receipt);
                return Task.FromResult(response);
            }

            private static Task<Response> PopulateEfrCustomerData(PopulateEfrCustomerDataRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<DataModelEFR.Documents.Receipt>(request.Receipt);
                return Task.FromResult(response);
            }

            private static Task<Response> PopulateCountryRegionSpecificPositions(PopulateCountryRegionSpecificPositionsRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<List<ReceiptPosition>>(request.ReceiptPositions);
                return Task.FromResult(response);
            }

            private static Task<Response> PopulateEfrLocalizationInfoToPayment(PopulateEfrLocalizationInfoToPaymentRequest request)
            {
                Response response = new SingleEntityDataServiceResponse<ReceiptPayment>(request.ReceiptPayment);
                return Task.FromResult(response);
            }
        }
    }
}