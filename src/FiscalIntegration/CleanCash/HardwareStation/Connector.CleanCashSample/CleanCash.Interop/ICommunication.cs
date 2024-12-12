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
    namespace HardwareStation.Connector.CleanCashSample.CleanCash.Interop
    {
        using System;
        using System.Runtime.CompilerServices;
        using System.Runtime.InteropServices;

        /// <summary>
        /// Provides the common communication interface to the CleanCash API.
        /// </summary>
        /// <remarks> All properties are only valid after <c>Open</c> method has been called.</remarks>
        [Guid("96E884EE-9854-48EA-B0AD-5E5B167C0FFE")]
        [TypeLibType(4160)]
        [ComImport]
        public interface ICommunication
        {
            /// <summary>
            /// A string describting API DLL.
            /// </summary>
            [DispId(1)]
            string ApiDescription { [DispId(1)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// A string in the format "x.y.z" which identifies the API version (e.g "1.0.132").
            /// </summary>
            [DispId(2)]
            string ApiVersion { [DispId(2)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// Returns the status of the CleanCash unit, after calling the property <c>LastUnitStatus</c> contains the same status.
            /// </summary>
            /// <returns>The result code.</returns>
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult CheckStatus();

            /// <summary>
            /// The result code for the last method call.
            /// </summary>
            [DispId(4)]
            CommunicationResult LastError { [DispId(4)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// Completes ongoing transaction and closes the communication.
            /// </summary>
            /// <returns>The result code.</returns>
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult Close();

            /// <summary>
            /// Must be called before checking any properties or calling methods.
            /// </summary>
            /// <param name="connectionString">A string containing parameters for the communication.</param>
            /// <returns>The result code.</returns>
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult Open([MarshalAs(UnmanagedType.BStr)][In] string connectionString);

            /// <summary>
            /// Must be called after <c>Open</c> to set organization number and POS id.
            /// </summary>
            /// <param name="organisationNumber">The company's registration number (10 digits)</param>
            /// <param name="posId">The unique POS id (16 characters)</param>
            /// <returns>The result code.</returns>
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult RegisterPos([MarshalAs(UnmanagedType.BStr)][In] string organisationNumber,
                [MarshalAs(UnmanagedType.BStr)][In] string posId);

            /// <summary>
            /// Used to register a receipt in the CleanCash unit.
            /// </summary>
            /// <param name="dateTime">Date/time od the receipt (YYYYMMDDhhmm).</param>
            /// <param name="receiptId">A unique receipt ID.</param>
            /// <param name="receiptType">Receipt type.</param>
            /// <param name="receiptTotal">The net sales amount including VAT.</param>
            /// <param name="negativeTotal">The total amount of negative items that are included in calculation of receipt total.
            /// <c>negativeTotal</c> should be an absolute value (i.e positive).</param>
            /// <param name="vat1">VAT 1 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <param name="vat2">VAT 2 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <param name="vat3">VAT 3 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <param name="vat4">VAT 4 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <returns>The result code.</returns>
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult SendReceipt(
                [MarshalAs(UnmanagedType.BStr)][In] string dateTime,
                [MarshalAs(UnmanagedType.BStr)][In] string receiptId,
                [In] CommunicationReceipt receiptType,
                [MarshalAs(UnmanagedType.BStr)][In] string receiptTotal,
                [MarshalAs(UnmanagedType.BStr)][In] string negativeTotal,
                [MarshalAs(UnmanagedType.BStr)][In] string vat1,
                [MarshalAs(UnmanagedType.BStr)][In] string vat2,
                [MarshalAs(UnmanagedType.BStr)][In] string vat3,
                [MarshalAs(UnmanagedType.BStr)][In] string vat4);

            /// <summary>
            /// Must be called before <c>SendReceipt</c>.
            /// </summary>
            /// <returns>The result code.</returns>
            [DispId(9)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult StartReceipt();

            /// <summary>
            /// The cashier's id (max 16 characters).
            /// </summary>
            [DispId(10)]
            string CashierId
            {
                [DispId(10)]
                [MethodImpl(MethodImplOptions.InternalCall)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [param: MarshalAs(UnmanagedType.BStr)]
                [param: In]
                set;
            }

            /// <summary>
            /// The connection string passed to <c>Open</c> method.
            /// </summary>
            [DispId(11)]
            string connectionString { [DispId(11)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// The control code for the last receipt.
            /// </summary>
            [DispId(12)]
            string LastControlCode { [DispId(12)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// The status of the CleanCash unit at the time for the last call <c>CheckStatus</c> or <c>CheckStatusEx</c>.
            /// </summary>
            [DispId(13)]
            CommunicationStatus LastUnitStatus { [DispId(13)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// If last call to <c>CheckStatus</c> or <c>CheckStatusEx</c> returned a value other than STATUS_OK or STATUS_UNKNOWN,
            /// then this property may contain a list of status codes response described by CleanCash Signature Protocol.
            /// </summary>
            [DispId(14)]
            string LastUnitStatusCodeList { [DispId(14)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }


            /// <summary>
            /// Extended error code for the last method call.
            /// </summary>
            [DispId(15)]
            int LastExtendedError { [DispId(15)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// The Swedish company registration number provided to <c>RegisterPos</c>.
            /// </summary>
            [DispId(16)]
            string organisationNumber { [DispId(16)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// The POS id provided to <c>RegisterPos</c>.
            /// </summary>
            [DispId(17)]
            string posId { [DispId(17)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// The CleanCash unit's manufacturing number (tillverkningsnummer).
            /// </summary>
            [DispId(18)]
            string UnitId { [DispId(18)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// The CleanCash unit's version inforamtion.
            /// </summary>
            [DispId(19)]
            string UnitVersion { [DispId(19)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            /// <summary>
            /// Used to register a receipt in the CleanCash unit.It is only supported by API 1.1 and later 
            /// and is used in applications where organization number/POS-id change often.
            /// It does not need to call<c>RegisterPos</c> before.
            /// </summary>
            /// <param name="organisationNumber">The company's registration number.</param>
            /// <param name="posId">The unique POS id.</param>
            /// <param name="dateTime">Date/time od the receipt (YYYYMMDDhhmm).</param>
            /// <param name="receiptId">A unique receipt ID.</param>
            /// <param name="receiptType">Receipt type.</param>
            /// <param name="receiptTotal">The net sales amount including VAT.</param>
            /// <param name="negativeTotal">The total amount of negative items that are included in calculation of receipt total.
            /// <c>negativeTotal</c> should be an absolute value (i.e positive).</param>
            /// <param name="vat1">VAT 1 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <param name="vat2">VAT 2 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <param name="vat3">VAT 3 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <param name="vat4">VAT 4 percentage rates and amounts (e.g 25,00;20.00).</param>
            /// <returns>The resut code.</returns>
            [DispId(20)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult SendReceiptEx(
                [MarshalAs(UnmanagedType.BStr)][In] string organisationNumber,
                [MarshalAs(UnmanagedType.BStr)][In] string posId,
                [MarshalAs(UnmanagedType.BStr)][In] string dateTime,
                [MarshalAs(UnmanagedType.BStr)][In] string receiptId,
                [In] CommunicationReceipt receiptType,
                [MarshalAs(UnmanagedType.BStr)][In] string receiptTotal,
                [MarshalAs(UnmanagedType.BStr)][In] string negativeTotal,
                [MarshalAs(UnmanagedType.BStr)][In] string vat1,
                [MarshalAs(UnmanagedType.BStr)][In] string vat2,
                [MarshalAs(UnmanagedType.BStr)][In] string vat3,
                [MarshalAs(UnmanagedType.BStr)][In] string vat4);

            [DispId(21)]
            int UnusedLicenses { [DispId(21)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// Information about the CleanCash unit (API 1.1).
            /// </summary>
            [DispId(22)]
            CommunicationUnitType UnitType { [DispId(22)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            [DispId(23)]
            CommunicationStorage LastStorageStatus { [DispId(23)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// The number of licenses installed in the CleanCash unit. Updated be <c>RegisterPOS</c>. (API 1.1)
            /// </summary>
            [DispId(24)]
            int LicensesInstalled { [DispId(24)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// Checks the status of the CleanCash control unit. This command should always be used for
            /// checking status when <c>SendReceiptEx</c> is used for sending receipts.
            /// </summary>
            /// <param name="organisationNumber">The company's registration number.</param>
            /// <param name="posId">The unique POS id.</param>
            /// <returns>The result code.</returns>
            [DispId(25)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            CommunicationResult CheckStatusEx(
                [MarshalAs(UnmanagedType.BStr)][In] string organisationNumber,
                [MarshalAs(UnmanagedType.BStr)][In] string posId);
        }
    }
}