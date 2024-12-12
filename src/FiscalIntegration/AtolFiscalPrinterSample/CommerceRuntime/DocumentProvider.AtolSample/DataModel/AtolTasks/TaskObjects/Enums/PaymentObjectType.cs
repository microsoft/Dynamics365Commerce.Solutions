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
    namespace CommerceRuntime.DocumentProvider.AtolSample.DataModel.AtolTask
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Payment object types supported by the device.
        /// </summary>
        [DataContract]
        public enum PaymentObjectType
        {
            /// <summary>
            /// The object of the payment is commodity.
            /// </summary>
            [EnumMember(Value = "commodity")]
            Commodity = 1,

            /// <summary>
            /// The object of the payment is excise.
            /// </summary>
            [EnumMember(Value = "excise")]
            Excise = 2,

            /// <summary>
            /// The object of the payment is job.
            /// </summary>
            [EnumMember(Value = "job")]
            Job = 3,

            /// <summary>
            /// The object of the payment is service.
            /// </summary>
            [EnumMember(Value = "service")]
            Service = 4,

            /// <summary>
            /// The object of the payment is gambling bet.
            /// </summary>
            [EnumMember(Value = "gamblingBet")]
            GamblingBet = 5,

            /// <summary>
            /// The object of the payment is gambling prize.
            /// </summary>
            [EnumMember(Value = "gamblingPrize")]
            GamblingPrize = 6,

            /// <summary>
            /// The object of the payment is lottery.
            /// </summary>
            [EnumMember(Value = "lottery")]
            Lottery = 7,

            /// <summary>
            /// The object of the payment is lottery prize.
            /// </summary>
            [EnumMember(Value = "lotteryPrize")]
            LotteryPrize = 8,

            /// <summary>
            /// The object of the payment is intellectual activity.
            /// </summary>
            [EnumMember(Value = "intellectualActivity")]
            IntellectualActivity = 9,

            /// <summary>
            /// The object of the payment is payment.
            /// </summary>
            [EnumMember(Value = "payment")]
            Payment = 10,

            /// <summary>
            /// The object of the payment is agent commission.
            /// </summary>
            [EnumMember(Value = "agentCommission")]
            AgentCommission = 11,

            /// <summary>
            /// The object of the payment is composite.
            /// </summary>
            [EnumMember(Value = "composite")]
            Composite = 12,

            /// <summary>
            /// The object of the payment is another.
            /// </summary>
            [EnumMember(Value = "another")]
            Another = 13,

            /// <summary>
            /// The object of the payment is proprietary law.
            /// </summary>
            [EnumMember(Value = "proprietaryLaw")]
            ProprietaryLaw = 14,

            /// <summary>
            /// The object of the payment is non operating income.
            /// </summary>
            [EnumMember(Value = "nonOperatingIncome")]
            NonOperatingIncome = 15,

            /// <summary>
            /// The object of the payment is insurance contributions.
            /// </summary>
            [EnumMember(Value = "insuranceContributions")]
            InsuranceContributions = 16,

            /// <summary>
            /// The object of the payment is merchant tax.
            /// </summary>
            [EnumMember(Value = "merchantTax")]
            MerchantTax = 17,

            /// <summary>
            /// The object of the payment is resort fee.
            /// </summary>
            [EnumMember(Value = "resortFee")]
            ResortFee = 18,

            /// <summary>
            /// The object of the payment is deposit.
            /// </summary>
            [EnumMember(Value = "deposit")]
            Deposit = 19,

            /// <summary>
            /// The object of the payment is consumption.
            /// </summary>
            [EnumMember(Value = "consumption")]
            Consumption = 20,

            /// <summary>
            /// The object of the payment is sole proprietor CPI contributions.
            /// </summary>
            [EnumMember(Value = "soleProprietorCPIContributions")]
            SoleProprietorCPIContributions = 21,

            /// <summary>
            /// The object of the payment is CPI contributions.
            /// </summary>
            [EnumMember(Value = "cpiContributions")]
            CpiContributions = 22,

            /// <summary>
            /// The object of the payment is sole proprietor CMI contributions.
            /// </summary>
            [EnumMember(Value = "soleProprietorCMIContributions")]
            SoleProprietorCMIContributions = 23,

            /// <summary>
            /// The object of the payment is CMI contributions.
            /// </summary>
            [EnumMember(Value = "cmiContributions")]
            CmiContributions = 24,

            /// <summary>
            /// The object of the payment is CSI contributions.
            /// </summary>
            [EnumMember(Value = "csiContributions")]
            CsiContributions = 25,

            /// <summary>
            /// The object of the payment is casino payment.
            /// </summary>
            [EnumMember(Value = "casinoPayment")]
            CasinoPayment = 26,
        }
    }
}
