# Extended Logon sample

## Overview
The original out-of-box code implementation for extended logon is not for direct usage, and extension and customization is required.\
This sample explains how to implement secure extended logon from end-to-end with second factor authentication by PIN number, including both POS and Commerce Runtime extensions. Through this sample, customers need to be aware of the security concern on credential length as well as the uniqueness of credential id.\
The sample covers extended logon in its whole lifecycle, including enrolling user credential, staff card logon or barcode logon, unlocking terminal and elevating user scenarios. In the above scenarios, POS users are required to input a PIN number besides the original credential to enhance security.

## Basic concepts of extended logon

### Grant type
Grant type describes the mechanism on how a POS user is authenticated.\
In normal operator Id and password logon, the grant type is "password".\
In staff card extended logon, the grant type is coded as "auth://example.auth.contoso.com/msr".\
In barcode extended logon, the grant type is coded as "auth://example.auth.contoso.com/barcode".

### Credential
A user credential is a string which is recorded in the physical staff card, which is scanned out when swiping the card in logon.\
Regarding the length of the credential, usually we need to ensure that all employees should have a unique credential. The second consideration is security, thus it should not be too short. Overall, we recommend the credential contains at least 256 bits to meet industry standard, which is 44 characters in Base64 encoding.

### Credential Id
A credential Id is an internal concept, usually it should be generated according to user credential and grant type, which is to uniquely identify the staff id per grant type.\
The out-of-box implementation is to fetch first 5 characters from the credential. This logic can be easily customized in the commerce runtime extensions. The maximum allowed length of credential id is 256.\
Please note that the credential id must be unique, otherwise enrolling error can happen randomly.

### Why second factor authentication is preferred?
With credential only, the magnetic card or barcode is easily duplicated. Furthermore, it can be lost or stealed and cause temporarily use.

## Project structure

### Pos folder
The Controls subfolder contains the sample dialog **PinInputDialog** which is used to collect PIN number from POS user.\
The Triggers subfolder contains pre-triggers **PreEnrollUserCredentialsTrigger**, **PreLogOnTrigger**, **PreUnlockTerminalTrigger** and **PreElevateUserTrigger**. These triggers' behaviors are similar - replace the trigger options with new extra parameter collected from **PinInputDialog**, and then pass the new options to the corresponding request. These triggers must work together when customer tries to introduce PIN number or any other second factor authentication method in extended logon.

### CommerceRuntime folder
**UniqueSecretExtendedAuthenticationService** is the abstract class of service request handler to process the extra authentication data in the extended logon, which is inherited by **BarcodeExtendedAuthenticationService** and **MagneticCardExtendedAuthenticationService**. Usually, you don't need to change code of these two inherited classes, only change the core logic in the abstract class. **UniqueSecretExtendedAuthenticationService** is responsible for the following 4 service requests.
- **GetUserAuthenticationCredentialIdServiceRequest** is used to calculate the credential id based on user credential and extra parameters. In this sample, we change the required length of credential and the length of credential id.
- **GetUserEnrollmentDetailsServiceRequest** is used in enrolling credential scenario, which calls previous **GetUserAuthenticationCredentialIdServiceRequest** to get enrollment details. In this sample, we do not change the behavior of this request and just delegate the execution to the default implementation.
- **OverrideUserCredentialServiceRequest** is used in both enrolling credential and validating logon token scenario, which is used to generate a new credential based on old credential and extra parameters dictionary. In this sample, we concat the credential and the PIN number to generate the new one.
- **ConfirmUserAuthenticationServiceRequest** is used to perform additional authentication check.  In this sample, we do not change the behavior of this request and just delegate the execution to the default implementation.

### ScaleUnit.Installer folder
It contains the scale unit installer project.

### StoreCommerce.Installer folder
It contains the store commerce app installer project.