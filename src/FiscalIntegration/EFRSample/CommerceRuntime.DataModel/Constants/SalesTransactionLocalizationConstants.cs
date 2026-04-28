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
    namespace CommerceRuntime.DocumentProvider.DataModelEFR.Constants
    {
        /// <summary>
        /// The sales transaction localization constants.
        /// </summary>
        public static class SalesTransactionLocalizationConstants
        {
            /// <summary>
            /// Customer order prepayment label.
            /// </summary>
            public const string CustomerDeposit = "CustomerDeposit";

            /// <summary>
            /// Deposit label.
            /// </summary>
            public const string Deposit = "Deposit";

            /// <summary>
            /// Deposit account label.
            /// </summary>
            public const string DepositAccount = "DepositAccount";

            /// <summary>
            /// Gift card label.
            /// </summary>
            public const string GiftCard = "GiftCard";

            /// <summary>
            /// Localization resource file name template.
            /// </summary>
            public const string LocalizationResourceFileNameTemplate = "{0}.Properties.Resources.Translations";

            /// <summary>
            /// The prefix of the localization string identifier.
            /// </summary>
            public const string LocalizationResourcePrefix = "Microsoft_Dynamics_Commerce_Runtime_DocumentProvider_EFR_";

            /// <summary>
            /// Training transaction.
            /// </summary>
            public const string TrainingTransaction = "TrainingTransaction";
        }
    }
}
