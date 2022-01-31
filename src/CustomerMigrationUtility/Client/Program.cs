// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Client
{
    using Microsoft.Dynamics.Commerce.CustomerMigrationUtility.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    /// <summary>The user migration.</summary>
    class Program
    {
        private static string tenant = ConfigurationManager.AppSettings["B2CTenant"];
        private static string clientId = ConfigurationManager.AppSettings["B2CClientId"];
        private static string clientSecret = ConfigurationManager.AppSettings["B2CClientSecret"];
        private static string nonInteractiveClientId = ConfigurationManager.AppSettings["NonInteractiveClientId"];
        private static string nonInteractivePolicyId = ConfigurationManager.AppSettings["NonInteractivePolicyId"];
        private static string scope = ConfigurationManager.AppSettings["Scope"];
        private static string loginDomain = ConfigurationManager.AppSettings["LoginDomain"];

        private static string password = ConfigurationManager.AppSettings["Password"];

        private static string inputFile = ConfigurationManager.AppSettings["InputFile"];

        private static string retailServerEndpoint = ConfigurationManager.AppSettings["RetailServerEndpoint"];
        private static string operatingUnitNumber = ConfigurationManager.AppSettings["OperatingUnitNumber"];

        private static readonly bool forceChangePasswordNextLogin = Convert.ToBoolean(ConfigurationManager.AppSettings["ForceChangePasswordNextLogin"]);
        private static readonly bool ResetPasswordInExistingAccount = Convert.ToBoolean(ConfigurationManager.AppSettings["ResetPasswordInExistingAccount"]);
        private static readonly bool disableTocreateAccountsInAX = Convert.ToBoolean(ConfigurationManager.AppSettings["DisableTocreateAccountsInAX"]);
        private static readonly bool createUserMappingInAX = Convert.ToBoolean(ConfigurationManager.AppSettings["CreateUserMappingInAX"]);

        private static B2CUserManager b2CUserManager;
        private static AXUserManager AXUserManager;

        static void Main(string[] args)
        {
            var logger = new Logger(false);
            var message = string.Empty;

            message = "Starting user migration...";
            Console.WriteLine(message);
            logger.Trace(message);

            b2CUserManager = new B2CUserManager(tenant, clientId, clientSecret);
            AXUserManager = new AXUserManager();

            if (!File.Exists(inputFile))
            {
                message = "Unanble to locale input file:" + inputFile;
                Console.WriteLine(message);
                logger.Error(message);
            }

            InputfromCSV();
        }

        /// <summary>Import the data from CSV.</summary>        
        /// <returns>The <see cref="ActionResult"/>.</returns>
        static void InputfromCSV()
        {
            var logger = new Logger();
            var message = string.Empty;
            List<B2CUser> usersFromInputFile = new List<B2CUser>();
         
            using (StreamReader r = new StreamReader(inputFile))
            {
                logger.Trace("Reading input file.");
                string line;
                int number = 0;
                int created = 0;
                int failed = 0;
                int skipped = 0;
                int disabled = 0;

                using (StreamWriter writer = new StreamWriter("MigrationStatus.csv", false))
                {
                    var logFormat = "#,B2CSTATUS,EXTERNALIDENTITYID,CUSTOMERACCOUNTNUMBER";

                    writer.WriteLine(logFormat);
                    string headerLine = r.ReadLine();

                    while ((line = r.ReadLine()) != null)
                    {
                        var items = line.Split(',');
                        B2CUser userFromInputFile = new B2CUser(items[1], items[2], tenant, items[0], items[3]);

                        if (items.Length == 7)
                        {
                            userFromInputFile.ExternalIssuer = items[4];
                            userFromInputFile.ExternalIssuerUserId = items[5];
                            password = items[6];
                        }
                        else
                        {
                            password = items[4];
                        }

                        userFromInputFile.TemporaryPassword = string.IsNullOrEmpty(password) ? b2CUserManager.CreatePassword(20) : password;

                        usersFromInputFile.Add(userFromInputFile);
                    }

                    foreach (B2CUser userFromInputFile in usersFromInputFile)
                    {
                        Response dyanmicsaccountStatus = new Response(Status.Skipped);


                        var oid = string.Empty;
                        var accountNumber = string.Empty;

                        number++;

                        LogToConsole("Number", number.ToString(), ConsoleColor.Yellow);
                        LogToConsole("E-Mail", userFromInputFile.EMail, ConsoleColor.Magenta);

                        // Creating B2C account
                        logger.Trace("Starting account creation in B2C.");

                        Response b2cAccountCreationStatus = b2CUserManager.CreateB2CAccount(userFromInputFile, ResetPasswordInExistingAccount).Result;

                        Console.Write("\t B2C : ");

                        if (b2cAccountCreationStatus.Status.ToString().Equals("Success", StringComparison.CurrentCultureIgnoreCase))
                        {
                            created++;
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            oid = b2cAccountCreationStatus?.UserId;
                        }
                        else if (b2cAccountCreationStatus.Status.ToString().Equals("Failed", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            failed++;                        
                        }
                        else
                        {
                            skipped++;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            oid = b2cAccountCreationStatus?.UserId;
                        }

                        if (b2cAccountCreationStatus.Status.ToString().Equals("Failed", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.WriteLine(b2cAccountCreationStatus.Status.ToString());
                        }
                        else
                        {
                            Console.Write(b2cAccountCreationStatus.Status.ToString());
                        }

                        Console.ResetColor();

                        if (createUserMappingInAX && !b2cAccountCreationStatus.Status.ToString().Equals("Failed", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.Write("\t AXCustomerLink : ");

                            Response linkStatus = AXUserManager.LinkAccount(userFromInputFile.CustomerAccountNumber, b2cAccountCreationStatus.UserId).Result;

                            if (linkStatus.Status.ToString().Equals("Success", StringComparison.CurrentCultureIgnoreCase))
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGreen;                            
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }

                            Console.WriteLine(linkStatus.Status.ToString());

                            Console.ResetColor();
                        }

                        var logValue = string.Format("{0},{1},{2},{3}",
                            number,
                            b2cAccountCreationStatus.Status.ToString(),
                            oid,
                            userFromInputFile.CustomerAccountNumber);

                        writer.WriteLine(logValue);
                    }
       
                    Console.WriteLine();
                    Console.WriteLine(string.Format("Total users in input file: {0}", number));
                    Console.WriteLine(string.Format("Total users created: {0}", created));
                    Console.WriteLine(string.Format("Total users skipped: {0}", skipped));
                    Console.WriteLine(string.Format("Total users disabled: {0}", disabled));
                    Console.WriteLine(string.Format("Total users failed to create or update: {0}", number - created - skipped));

                    message = "User migration is complete.";
                    Console.WriteLine(message);
                    logger.Trace(message);
                }
            }
        }

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="title">The title of the message to log.</param>
        /// <param name="value">The value of the message to log.</param>
        /// <param name="color">The color of the message.</param>
        private static void LogToConsole(string title, string value, ConsoleColor color)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Console.Write($"\t {title} : ");
            }

            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ResetColor();
        }
    }
}
