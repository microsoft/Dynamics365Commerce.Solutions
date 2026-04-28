# Dynamics 365 Commerce Customer Migration Tool
**Important:** Due to the nature of data being handled for migration, it is highly recommended to set up and execute the following Tooling only on machines and networks that meet your company’s security protocol for sensitive information.

The tool provided exhibits how an Azure Application can be used to make Graph API calls to populate AAD B2C and generate a linking reference in Dynamics Commerce to the customer account records. If starting with the customer data export from Dynamics Commerce, the exported data can be transformed and used with the migration tool to create B2C Records. Those records must then be linked in Dynamics to connect the created B2C record with the existing customer account record.


## Configuration Steps for Graph API
Graph API will be used to read & create the user accounts in B2C tenant.

To use the Graph API with your B2C tenant, you need to register an application using the App Registrations service in the Azure portal, NOT Azure AD B2C's Applications menu. The following instructions lead you to the appropriate menu. You can't reuse existing B2C applications that you registered in the Azure AD B2C's Applications menu.

### Create the Application:

1. Sign in to the Azure portal.
2. Choose your Azure AD B2C tenant by selecting your account in the top right corner of the page.
3. In the left-hand navigation pane, choose **All Services**, click **App Registrations**, and click **Add**.
4. Follow the prompts and create a new application.
    1. Select **Web App/API** as the Application Type.
    2. Provide **any Sign-on URL** (e.g. https://B2CGraphAPI) as it's not relevant for this example
Provide any Sign-on URL (e.g. https://B2CGraphAPI) as it's not relevant for this example.

5. The application will now show up in the list of applications, click on it to obtain the **Application ID** (also known as Client ID). Copy it as you'll need it for **“B2CClientId”** in user migration configuration.
6. In the Settings menu, click **Keys**.
7.	In the **Passwords** section, enter the key description and select a duration, and then click **Save**. Copy it as you will need it for **“B2CClientSecret”** in user migration configuration.

### Update permissions for the application:
1.	Continuing in the Azure portal's App Registrations menu, select your application.

2.	In the Settings menu, click on **Required permissions**.
3.	In the Required permissions menu, click on **Windows Azure Active Directory**.
4.	In the Enable Access menu, select the **Read and write directory data** permission from Application Permissions and click Save.
5.	Finally, back in the Required permissions menu, click on the Grant Permissions button.

## Setting Up Data Permissions for HQ Customer Linking
This section details setting up data permissions to enable the Migration Tool to link created AAD B2C records to the customer records in Dynamics. This functionality is only available in Retail versions **10.0.10** and above.

1.	Go to **https://portal.azure.com** and select the correct AAD
2.	Next, go to Azure Active Directory > **App registrations** > and click on **“New application registration”**.
3.	Enter a name, select Web app/API, and enter your HQ URL for sign on.
4.	Go back to the list of applications and choose the newly created app. Click on the app and go to “required permissions”.
5.	Select **“Microsoft Dynamics ERP”** and give access permissions.
6. 	From the registered app settings page go to **Keys** and create a **new key**. Copy the key value.
7. 	Open **D365 HQ** and go to System **Administration** > **Setup** > **Azure Active Directory applications**.
8. Add **Client Id** with "Admin" user id.

## Collect the required Information for Record Linking
If **Customer records already exist** in Dynamics, the Migration Tool will link the newly created B2C record to the Customer Account Number. For this, the **ProviderID** and **DataAreaID** need to be configured in the migration tool config file.  

1. **ProviderID** – the internal Dynamics reference for the AAD B2C tenant configured in HQ and associated to Site Builder
2. **DataAreaID** – the reference to the Legal Entity in Dynamics which the customer account falls under

 **IMPORTANT**: Only **import a set of customers per a Legal Entity and AAD Tenant** at a time with this configuration in the Migration Tool. These two items must be accurate to properly link a customer record to the right AAD Tenant and Legal Entity within Dynamics. Be sure to update the **UserMigration.exe.config** for any different customer migration pertaining to Legal Entity or AAD B2C tenant differences.

To collect the Data Area ID and Provider ID, in Dynamics 365 HQ, there are two options: 

#### Option 1:

1.	Do one test sign-in through your site for your Channel to create a referenceable record in the table for the environment and channel.
2.	Reference the customer number for this record in HQ for this record.
3.	In your browser, go to your **HQ URL**
4.	Authenticate when prompted
5.	Now go to your **‘<HQ URL>/data/ExternalIdToCustomerMaps’**
6.	Find your initial customer number referenced in steps A and B, and use the **ProviderID** and **DataAreaID** for this record.

Once collected, you can update the following values in the UserMigrationUtility.exe.config file:
```XML
<add key="ProviderId" value="**PROVIDERID**"/>
<add key="DataAreaId" value="**DATAAREAID**"/>
<add key="CreateUserMappingInAX " value="true"/>
```
#### Option 2:
Use the ‘Customer External Identifier’ entity which can be exported with an initial record for reference.
1.	Perform an initial test sign-up through your site for your Channel to create a referenceable record in the table for the environment and channel.
2.	Reference the customer number for this record in HQ for this record.
3.	In HQ, go to Data Management> Data Export and create a job with the ‘Customer External Identifier’ within the Legal Entity (this entity is also referenced with **RETAILEXTERNALIDTOCUSTOMERMAP** as the target entity). Note: a CSV format is recommended to avoid issues with leading zeros in the data.
4.	Export the job and download the reference file.
5.	Reference the **ProviderID** and the **DataAreaID** from the dataset. Ensure you are referencing the intended Customer Number from the initial sign up in step A. These Provider ID and DataAreaID values will be the Dynamics referenced ID for the B2C tenant mapped in the HQ Identity Provider table, and the data area associated to the customer within the legal entity respectively.

Once collected, you can update the following values in the UserMigrationUtility.exe.config file:
```XML
 <add key="ProviderId" value="PROVIDERID"/>
 <add key="DataAreaId" value="DATAAREAID"/>
 <add key="CreateUserMappingInAX " value="true"/>
```

## Migration Steps

1.	Copy the sent Migration Tool Zip folder and extract to a new folder.
2.	Go to folder and update **UserMigrationUtility.exe.config** with the appropriate configuration.
3.	Add/Update user details in **CustomerData.csv**. Use the same format as the sample provided. Last 2 columns are needed for social accounts migration only.
4.	Execute **UserMigrationUtility.exe** in the command prompt. The tool will write the status in **MigrationStatus.csv**.

Below is an example of the configuration file with sample-data only. Use your relevant values for your AAD, applications, and policies:
```XML
  <appSettings>
    <!-- Graph API configuration -->
    <add key="B2CTenant" value="" />
    <add key="B2CClientId" value="" />
    <add key="B2CClientSecret" value="" />
    <add key="PropertyExtension" value="" />
    <add key="InputFile" value="CustomerData.csv" />
    
     <!-- AX Odata Client & AAD configuration -->
    <add key="CreateUserMappingInAX" value="true" />
    <add key="AX-AOS-Url" value="" />
    <add key="AX-AAD-Tenant" value="" />
    <add key="AX-AAD-ClientId" value="" />
    <add key="AX-AAD-ClientPassword" value="" />
    <add key="ProviderId" value="" />
    <add key="DataAreaId" value="" />
```