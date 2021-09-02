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
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Constants
    {
        /// <summary>
        /// Contains constants of attributes for fiscal integration document.
        /// </summary>
        /// <remark>
        /// This is defined by a third-party library, or by the tax authority, etc.
        /// </remark>
        public static class DocumentAttributeConstants
        {
            /// <summary>
            /// The adjustmentType attribute.
            /// </summary>
            internal const string AdjustmentType = "adjustmentType";

            /// <summary>
            /// The amount attribute.
            /// </summary>
            internal const string Amount = "amount";

            /// <summary>
            /// The value for barcode type used for codeType attribute.
            /// </summary>
            internal const string BarCodeTypeCODE128 = "CODE128";

            /// <summary>
            /// The cash tender type for payment type attribute.
            /// </summary>
            internal const string Cash = "Cash";

            /// <summary>
            /// The cheque tender type for payment type attribute.
            /// </summary>
            internal const string Cheque = "Cheque";

            /// <summary>
            /// The code attribute.
            /// </summary>
            internal const string Code = "code";

            /// <summary>
            /// The codeType attribute.
            /// </summary>
            internal const string CodeType = "codeType";
            /// <summary>
            /// The comment attribute.
            /// </summary>
            internal const string Comment = "comment";

            /// <summary>
            /// The credit or credit card tender type for payment type attribute.
            /// </summary>
            internal const string CreditOrCreditCard = "Credit or credit card";

            /// <summary>
            /// The data attribute.
            /// </summary>
            internal const string Data = "data";

            /// <summary>
            /// The dispaly value of attribute data.
            /// </summary>
            internal const string DataValue = "Message on customer display";

            /// <summary>
            /// The default value for department attribute.
            /// </summary>
            internal const string DefaultDepartment = "1";

            /// <summary>
            /// The default value for height attribute.
            /// </summary>
            internal const string DefaultHeight = "88";

            /// <summary>
            /// The default value for hRIFont attribute.
            /// </summary>
            internal const string DefaultHRIFont = "A";

            /// <summary>
            /// The default value for hRIPosition attribute.
            /// </summary>
            internal const string DefaultHRIPosition = "1";

            /// <summary>
            /// The default value for index attribute of cash payment type.
            /// </summary>
            internal const string DefaultIndexForCash = "0";

            /// <summary>
            /// The default value for index attribute of credit or credit card and ticket payment type.
            /// </summary>
            internal const string DefaultIndexForCreditAndTicket = "1";

            /// <summary>
            /// The default value for index attribute of credit memo payment type.
            /// </summary>
            internal const string DefaultIndexForCreditMemo = "0";

            /// <summary>
            /// The default value for justification attribute.
            /// </summary>
            internal const string DefaultJustification = "2";

            /// <summary>
            /// The default value for message type attribute.
            /// </summary>
            internal const string DefaultMessageType = "4";

            /// <summary>
            /// The default value for operator attribute.
            /// </summary>
            internal const string DefaultOperatorId = "1";

            /// <summary>
            /// The default operator ID for the tax code direct IO command.
            /// </summary>
            internal const string DefaultTaxCodeOperatorId = "01";

            /// <summary>
            /// The default description for discount on payment.
            /// </summary>
            internal const string DefaultPaymentDescriptionForDiscount = "Payment discount";

            /// <summary>
            /// The default PaymentIndex value for Deposit payment method.
            /// </summary>
            internal const string DefaultPaymentIndexForDepositPayment = "00";

            /// <summary>
            /// The default PaymentIndex value for discount on payment.
            /// </summary>
            internal const string DefaultPaymentIndexForDiscount = "00";

            /// <summary>
            /// The default PaymentIndex value for PayCard operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayCard = "00";

            /// <summary>
            /// The default PaymentIndex value for PayCash operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayCash = "00";

            /// <summary>
            /// The default PaymentIndex value for PayCheck operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayCheck = "00";

            /// <summary>
            /// The default PaymentIndex value for PayCreditMemo operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayCreditMemo = "01";

            /// <summary>
            /// The default PaymentIndex value for PayCurrency operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayCurrency = "00";

            /// <summary>
            /// The default PaymentIndex value for PayCustomerAccount operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayCustomerAccount = "00";

            /// <summary>
            /// The default PaymentIndex value for PayGiftCard operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayGiftCard = "01";

            /// <summary>
            /// The default PaymentIndex value for PayLoyaltyCard operation.
            /// </summary>
            internal const string DefaultPaymentIndexForPayLoyaltyCard = "00";

            /// <summary>
            /// The default PaymentType value for Deposit payment method.
            /// </summary>
            internal const string DefaultPaymentTypeForDepositPayment = "2";

            /// <summary>
            /// The default PaymentType value for discount on payment.
            /// </summary>
            internal const string DefaultPaymentTypeForDiscount = "6";

            /// <summary>
            /// The default PaymentType value for PayCard operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayCard = "2";

            /// <summary>
            /// The default PaymentType value for PayCash operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayCash = "0";

            /// <summary>
            /// The default PaymentType value for PayCheck operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayCheck = "1";

            /// <summary>
            /// The default PaymentType value for PayCreditMemo operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayCreditMemo = "4";

            /// <summary>
            /// The default PaymentType value for PayCurrency operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayCurrency = "0";

            /// <summary>
            /// The default PaymentType value for PayCustomerAccount operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayCustomerAccount = "2";

            /// <summary>
            /// The default PaymentType value for PayGiftCard operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayGiftCard = "6";

            /// <summary>
            /// The default PaymentType value for PayLoyaltyCard operation.
            /// </summary>
            internal const string DefaultPaymentTypeForPayLoyaltyCard = "6";

            /// <summary>
            /// The default value for position attribute.
            /// </summary>
            internal const string DefaultPosition = "80";

            /// <summary>
            /// The default value for width attribute.
            /// </summary>
            internal const string DefaultWidth = "2";

            /// <summary>
            /// The department attribute.
            /// </summary>
            internal const string Department = "department";

            /// <summary>
            /// The description attribute.
            /// </summary>
            internal const string Description = "description";

            /// <summary>
            /// The default display value of attribute description in salesline discount line.
            /// </summary>
            internal const string DiscountAppliedResourceId = "FiscalReceiptItaly_DiscountApplied";

            /// <summary>
            /// The code for discount payment attribute.
            /// </summary>
            internal const string DiscountPaymentCode = "DiscountPayment";

            /// <summary>
            /// The font attribute.
            /// </summary>
            internal const string Font = "font";

            /// <summary>
            /// The height attribute.
            /// </summary>
            internal const string Height = "height";

            /// <summary>
            /// The hRIFont attribute.
            /// </summary>
            internal const string HRIFont = "hRIFont";

            /// <summary>
            /// The hRIPosition attribute.
            /// </summary>
            internal const string HRIPosition = "hRIPosition";

            /// <summary>
            /// The index attribute.
            /// </summary>
            internal const string Index = "index";

            /// <summary>
            /// The item product type value.
            /// </summary>
            internal const string ItemProductTypeValue = "0";

            /// <summary>
            /// The justification attribute.
            /// </summary>
            internal const string Justification = "justification";

            /// <summary>
            /// Follow the fiscal printer development guide of Epson FP90III,
            /// the max length for fiscal receipt attribute "description" and "message"(with messageType 4) is 37 characters.
            /// </summary>
            internal const int MaxAttributeLength = 37;

            /// <summary>
            /// Follow the fiscal printer development guide of Epson FP90III,
            /// the max length for fiscal receipt attribute "description" and "message"(with messageType 1) is 45 characters.
            /// </summary>
            internal const int MaxHeaderAttributeLength = 45;

            /// <summary>
            /// The message attribute.
            /// </summary>
            internal const string Message = "message";

            /// <summary>
            /// The message type attribute.
            /// </summary>
            internal const string MessageType = "messageType";

            /// <summary>
            /// The operator attribute.
            /// </summary>
            internal const string Operator = "operator";

            /// <summary>
            /// The payment attribute.
            /// </summary>
            internal const string Payment = "payment";

            /// <summary>
            /// The paymentType attribute.
            /// </summary>
            internal const string PaymentType = "paymentType";

            /// <summary>
            /// The text P.IVA' followed by VAT number
            /// </summary>
            internal const string PIVA = "P.IVA";

            /// <summary>
            /// The position attribute.
            /// </summary>
            internal const string Position = "position";

            /// <summary>
            /// The value for adjustmentType attribute in PrintRecItemAdjustment element.
            /// </summary>
            internal const string PrintRecItemAdjustmentTypeValue = "0";

            /// <summary>
            /// The value for OMAGGIO adjustmentType attribute in PrintRecItemAdjustment element.
            /// </summary>
            internal const string PrintRecItemOmaggioAdjustmentTypeValue = "11";

            /// <summary>
            /// The value for adjustmentType attribute in PrintRecSubTotalAdjustment element.
            /// </summary>
            internal const string PrintRecSubTotalAdjustmentTypeValue = "1";

            /// <summary>
            /// The quantity attribute.
            /// </summary>
            internal const string Quantity = "quantity";

            /// <summary>
            /// The text for description attribute of printRecRefund element.
            /// </summary>
            internal const string RefundGoodsReturn = "Refund item (goods return)";

            /// <summary>
            /// The service product type value.
            /// </summary>
            internal const string ServiceProductTypeValue = "1";

            /// <summary>
            /// The space separator.
            /// </summary>
            internal const string Space = " ";

            /// <summary>
            /// The ticket tender type for payment type attribute.
            /// </summary>
            internal const string Ticket = "Ticket";

            /// <summary>
            /// The timeOut attribute.
            /// </summary>
            internal const string TimeOut = "timeOut";

            /// <summary>
            /// The default value for specific attribute.
            /// </summary>
            internal const string Three = "3";

            /// <summary>
            /// The unit price attribute.
            /// </summary>
            internal const string UnitPrice = "unitPrice";

            /// <summary>
            /// The width attribute.
            /// </summary>
            internal const string Width = "width";

            /// <summary>
            /// The MessageType to print data in receipt header.
            /// </summary>
            internal const string ReceiptHeaderMessageType = "1";

            /// <summary>
            /// Number of lines that can be filled by company name.
            /// </summary>
            internal const int ReceiptFieldCompanyLinesLimit = 2;

            /// <summary>
            /// Number of lines that can be filled by company address.
            /// </summary>
            internal const int ReceiptFieldStoreAddressLinesLimit = 2;

            /// <summary>
            /// Number of lines that can be filled by customer name.
            /// </summary>
            internal const int ReceiptFieldCustomerNameLinesLimit = 2;

            /// <summary>
            /// Number of lines that can be filled by customer name.
            /// </summary>
            internal const int ReceiptHeaderTotalLinesLimit = 9;

            /// <summary>
            /// The command attribute.
            /// </summary>
            internal const string Command = "command";

            /// <summary>
            /// The vat id attribute.
            /// </summary>
            internal const string VatId = "vatID";
        }
    }
}
