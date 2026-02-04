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
        /// Class implements sample of integration with Clean Cash fiscal register.
        /// </summary>
        internal class CleanCashFiscalRegister : IDisposable
        {
            private const string ConnectionStringIsNotSpecifiedErrorMessage = "Fiscal register connection string is not specified.";
            private const string CleanCashCommunicationErrorMessage = "There was an error communicating with the fiscal register.";

            private Communication cleanCashInteropDriver;

            private string connectionString;

            internal CleanCashFiscalRegister(string connectionString)
            {
                this.connectionString = connectionString;
            }

            /// <summary>
            /// Checks if fiscal register is ready for registration operation or not.
            /// </summary>
            /// <returns><c>True</c> if fiscal register is ready, <c>false</c> otherwise.</returns>
            internal bool IsReady()
            {
                this.OpenCleanCashUnit();
                this.CloseCleanCashUnit();
                return true;
            }

            /// <summary>
            /// Registers fiscal transaction.
            /// </summary>
            /// <param name="fiscalTransactionData">Fiscal transaction data.</param>
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
            /// Dispose the clean cash and mutex object if the clean cash driver is not closed correctly.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Dispose the clean cash  objects if disposing is set to true.
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
            /// Instantiates and initializes Clean Cash control unit driver instance.
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
            /// Closes Clean Cash control unit.
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
            /// Checks result of control unit operation for any errors.
            /// </summary>
            /// <param name="resultCode">Result code.</param>
            private void CheckResultCode(CommunicationResult resultCode)
            {
                if (resultCode != CommunicationResult.RC_SUCCESS)
                {
                    this.CloseCleanCashUnit(true);
                    throw new CleanCashDeviceException(resultCode, CleanCashCommunicationErrorMessage);
                }
            }

            /// <summary>
            /// Disposes the Clean Cash control unit driver COM object instance.
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
