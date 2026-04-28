# CSU Application authentication for Store Checkout

## Overview
POS operations such as create shifts and add product for checkout are normally conducted by a worker, not an application.
In this sample code, it shows with using extension to enable application authentication to do:\
a. Automatically create shift for current date and close open shifts created in previous dates, in Channel's time zone.\
b. Add device information to Commerce principal so that the checkout can be done.

## Basic concepts of application authentication
CSU support several authentication, namely Customer, Worker, Application, And Anonymous.
For Customer, Worker, Application, we can configure at HQ "Commerce Shared parameter" page in the tab identity provider.
For more detail, please take a look at this [public doc](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/configure-authentication-providers)

### ShiftAndDeviceTrigger folder
**ShiftAndDeviceTrigger.cs** is the implementation to ensure commerce runtime have correct device information.
And it does auto-shift management using shift id as 'yyyyMMdd' for the corresponding channel timezone.\
**Information**: how to override the way to find the 1st device, please see the method: Get1stDeviceInformation\
There is a default way by specifying the value in extension commerce configuration for the key "ext.DeviceNumberPrefix
", as the result it will always look for the DeviceNumber with the prefix.\
**Warning**: create a new shift by POS on the same device may not align with this shift id format.\
Please resume the existing shift of the 1st device in the corresponding channel for any operations.

### ScaleUnit.Installer folder
It contains the scale unit installer project.