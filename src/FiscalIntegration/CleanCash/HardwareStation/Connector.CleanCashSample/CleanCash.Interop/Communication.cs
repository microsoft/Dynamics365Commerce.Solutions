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
        /// Provides the common communication to the CleanCash API.
        /// </summary>
        [Guid("48222505-45D6-4B3D-B586-611A5FF849A1")]
        [ClassInterface(ClassInterfaceType.None)]
        [ComImport]
        class Communication : ICommunication
        {
            public virtual extern string ApiDescription { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern string ApiVersion { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult CheckStatus();

            public virtual extern CommunicationResult LastError { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult Close();

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult Open([MarshalAs(UnmanagedType.BStr)][In] string connectionString);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult RegisterPos(
                [MarshalAs(UnmanagedType.BStr)][In] string organisationNumber,
                [MarshalAs(UnmanagedType.BStr)][In] string posId);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult SendReceipt(
                [MarshalAs(UnmanagedType.BStr)][In] string dateTime,
                [MarshalAs(UnmanagedType.BStr)][In] string receiptId,
                [In] CommunicationReceipt receiptType,
                [MarshalAs(UnmanagedType.BStr)][In] string receiptTotal,
                [MarshalAs(UnmanagedType.BStr)][In] string negativeTotal,
                [MarshalAs(UnmanagedType.BStr)][In] string vat1,
                [MarshalAs(UnmanagedType.BStr)][In] string vat2,
                [MarshalAs(UnmanagedType.BStr)][In] string vat3,
                [MarshalAs(UnmanagedType.BStr)][In] string vat4);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult StartReceipt();

            public virtual extern string CashierId
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [param: MarshalAs(UnmanagedType.BStr)]
                [param: In]
                set;
            }

            public virtual extern string connectionString { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern string LastControlCode { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern CommunicationStatus LastUnitStatus { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public virtual extern string LastUnitStatusCodeList { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern int LastExtendedError { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public virtual extern string organisationNumber { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern string posId { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern string UnitId { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            public virtual extern string UnitVersion { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult SendReceiptEx(
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

            public virtual extern int UnusedLicenses { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public virtual extern CommunicationUnitType UnitType { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public virtual extern CommunicationStorage LastStorageStatus { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public virtual extern int LicensesInstalled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public virtual extern CommunicationResult CheckStatusEx(
                [MarshalAs(UnmanagedType.BStr)][In] string organisationNumber,
                [MarshalAs(UnmanagedType.BStr)][In] string posId);
        }
    }
}