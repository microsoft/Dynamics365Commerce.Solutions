# Azure B2C Custom Policy setup
**Important:** A custom policy is fully configurable and policy-driven. A custom policy orchestrates trust between entities in standard protocols. For example, OpenID Connect, OAuth, SAML, and a few non-standard ones, for example REST API-based system-to-system claims exchanges. The framework creates user-friendly, white-labeled experiences.

A custom policy is represented as one or more XML-formatted files, which refer to each other in a hierarchical chain. The XML elements define the building blocks, the interaction with the user, and other parties, and the business logic.

### Pre-requisite steps :

1. Open the Start menu on your computer and search for **"PowerShell"** and Right-click on Windows PowerShell and choose **"Run as administrator"**.
2. Run **“Install-Module -Name AzureAD”** and press enter. Answer **"Yes"** if it is required repository update.
3. Copy **B2C tenant name** without ".onmicrosoft.com" from Azure portal.
4. Copy Entire **CustomPolicySetup** folder and go to installation folder and run **“PreInstallation.ps1 B2CTENANT”** with **Azure AD B2C global administrator** account. The script will automatically create Azure AD applications, provide the permissions, update custom policy files and update user migration configuration file with the required configuration. The script will create the configuration file with all core configuration for the future reference..

5. Open https://portal.Azure.com and go to **Azure AD B2C | App registrations (Preview)** and select **“ProxyIdentityExperienceFramework”**.
6.	Select **“API Permissions”** and Click **“Grant admin consent for (your tenant name)”**.
7. Add signing and encryption keys , select **Policies -> Identity Experience Framework.**
    1. Create the **signing key**:
       1.  Select Policy Keys and then select **Add**. For Options, choose **Generate**.
       2.  In Name, enter **TokenSigningKeyContainer**. The prefix B2C_1A_ might be added automatically.
       3.  For Key type, select **RSA**. For Key usage, select **Signature**. Select **Create**.

    2. Create the **encryption key**.
       1.  Select Policy Keys and then select **Add**. For Options, choose **Generate**.
       2.  In Name, enter **TokenEncryptionKeyContainer**. The prefix B2C_1A_ might be added automatically.
       3.  For Key type, select **RSA**. For Key usage, select **Encryption**. Select **Create**.

8. Update **custom UI Urls** , Open TrustFrameworkExtensions.xml and update custom Urls.
    1. Go to ContentDefinition Id="api.signuporsignin" section and reply url with the following format.
         <LoadUri>https://www.dynamics.com/en/{OAUTH-KV:ui_locales}/sign-in?preloadscripts=true</LoadUri>.
    2. Go to ContentDefinition Id=" api.selfasserted.profileupdate" section and reply url with the following format.
         <LoadUri>https://www.dynamics.com/en/{OAUTH-KV:ui_locales}/editprofile?preloadscripts=true</LoadUri>.
    3. Go to ContentDefinition Id="api.localaccountpasswordreset" section and reply url with the following format.
         <LoadUri>https://www.dynamics.com/en/{OAUTH-KV:ui_locales}/passwordreset?preloadscripts=true</LoadUri>.   
    4. Go to ContentDefinition Id="api. localaccountsignup" section and reply url with the following format.
         <LoadUri>https://www.dynamics.com/en/{OAUTH-KV:ui_locales}/sign-up?preloadscripts=true</LoadUri>.

9. Select **Identity Experience Framework**, select **Upload custom policy** and upload the policy files in the following order.
    1. TrustFrameworkBase.xml
    2. TrustFrameworkExtensions.xml
    3. SignUpOrSignin.xml
    4. ProfileEdit.xml
    5. PasswordReset.xml
