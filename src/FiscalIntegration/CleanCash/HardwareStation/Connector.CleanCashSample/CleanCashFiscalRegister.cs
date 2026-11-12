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
    namespace HardwareStation.Connector.CleanCashSample
    {
        using System;
        using System.Runtime.InteropServices;
        using CleanCash.Interop;

        /// <summary>
        /// Implements a sample integration with the CleanCash fiscal register.
        /// </summary>
        internal class CleanCashFiscalRegister : IDisposable
        {
            private const string ConnectionStringIsNotSpecifiedErrorMessage = "Fiscal register connection string is not specified.";
            private const string CleanCashCommunicationErrorMessage = "There was an error communicating with the fiscal register.";

            private Communication cleanCashInteropDriver;

            private readonly string connectionString;

            /// <summary>
            /// Initializes a new instance of the <see cref="CleanCashFiscalRegister" /> class.
            /// </summary>
            /// <param name="connectionString">The fiscal register connection string.</param>
            internal CleanCashFiscalRegister(string connectionString) => this.connectionString = connectionString;

            /// <summary>
            /// Checks whether the fiscal register is ready for the registration operation.
            /// </summary>
            /// <returns><c>True</c> if the fiscal register is ready; otherwise, <c>false</c>.</returns>
            internal bool IsReady()
            {
                this.OpenCleanCashUnit();
                this.CloseCleanCashUnit();
                return true;
            }

            /// <summary>
            /// Registers the fiscal transaction.
            /// </summary>
            /// <param name="fiscalTransactionData">The fiscal transaction data.</param>
            /// <returns>The results of the fiscal registration.</returns>
            internal CleanCashResponse RegisterFiscalTransaction(CleanCashFiscalTransactionData fiscalTransactionData)
            {
                this.OpenCleanCashUnit();

                this.CheckResultCode(this.cleanCashInteropDriver.RegisterPos(fiscalTransactionData.StoreTaxRegNumber, fiscalTransactionData.TerminalId));
                this.CheckResultCode(this.cleanCashInteropDriver.StartReceipt());
                this.CheckResultCode(
                    this.cleanCashInteropDriver.SendReceipt(
                        fiscalTransactionData.TransactionDate,
                        fiscalTransactionData.ReceiptId,
                        fiscalTransactionData.IsCopy ? CommunicationReceipt.RECEIPT_COPY : CommunicationReceipt.RECEIPT_NORMAL,
                        fiscalTransactionData.TotalAmount,
                        fiscalTransactionData.NegativeAmount,
                        fiscalTransactionData.VAT1,
                        fiscalTransactionData.VAT2,
                        fiscalTransactionData.VAT3,
                        fiscalTransactionData.VAT4));
                return new CleanCashResponse(this.cleanCashInteropDriver.UnitId, this.cleanCashInteropDriver.LastControlCode);
            }

            /// <summary>
            /// Disposes the CleanCash and mutex objects if the CleanCash driver is not closed correctly.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Disposes the CleanCash objects if disposing is set to true.
            /// </summary>
            /// <param name="disposing">Disposing flag set to true.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.CloseCleanCashUnit(true);
                }
            }

            /// <summary>
            /// Instantiates and initializes the CleanCash control unit driver instance.
            /// </summary>
            private void OpenCleanCashUnit()
            {
                if (this.cleanCashInteropDriver == null)
                {
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new ArgumentNullException(nameof(connectionString), ConnectionStringIsNotSpecifiedErrorMessage);
                    }

                    this.cleanCashInteropDriver = new Communication();

                    this.CheckResultCode(this.cleanCashInteropDriver.Open(connectionString));
                    this.CheckResultCode(this.cleanCashInteropDriver.CheckStatus());
                }
            }

            /// <summary>
            /// Closes the CleanCash control unit.
            /// </summary>
            /// <param name="skipValidation">Skip the closing command result validation.</param>
            private void CloseCleanCashUnit(bool skipValidation = false)
            {
                if (this.cleanCashInteropDriver != null)
                {
                    CommunicationResult resultCode = this.cleanCashInteropDriver.Close();

                    if (!skipValidation)
                    {
                        this.CheckResultCode(resultCode);
                    }

                    this.DisposeCleanCashDriverInstance();
                }
            }

            /// <summary>
            /// Checks the result of the control unit operation for any errors.
            /// </summary>
            /// <param name="resultCode">The result code.</param>
            private void CheckResultCode(CommunicationResult resultCode)
            {
                if (resultCode != CommunicationResult.RC_SUCCESS)
                {
                    this.CloseCleanCashUnit(true);
                    throw new CleanCashDeviceException(resultCode, CleanCashCommunicationErrorMessage);
                }
            }

            /// <summary>
            /// Disposes the CleanCash control unit driver COM object instance.
            /// </summary>
            private void DisposeCleanCashDriverInstance()
            {
                if (this.cleanCashInteropDriver != null)
                {
                    Marshal.ReleaseComObject(this.cleanCashInteropDriver);
                    this.cleanCashInteropDriver = null;
                }
            }
        }
    }
}
