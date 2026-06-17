# Manage Your Education and Skills Funding Admin Api

The Manage Your Education and Skills Funding (MYESF) Admin Api is used by the MYESF web application to allow the following:

- Retrieval of GOV.UK Notify dependent information for the sending of email notifications
- Storage and retrieval of impersonation provider information for internal DfE users

It will be enhanced to include any generic/common functionality which is required in the future.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 8 web api utilising Azure App Service for deployment.

The web api runs on an Azure App service on Azure.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `appsettings.json` file will need to be created in the `Pds.Admin.Api` project. Below, and included in the repo, there is `appsettings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`appsettings.json`)

```json
{
  "AzureAd": {
    "Audience": "",
    "ClientId": "",
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": ""
  },
  "Environment": "local",
  "Logging": {
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Error"
      }
    },
    "LogLevel": {
      "Default": "Information"
    }
  },
  "NotifyTableStorage:ConnectionString": "",
  "PdsApplicationInsights": {
    "InstrumentationKey": "",
    "Environment": "local"
  },
  "StorageCache": {
    "ConnectionString": "",
    "ItemLifetimeInMinutes": "480"
  }
}
```

### Setting Details

- **`AzureAd:Audience`**  
  The intended recipient of the azure authentication token.
 
- **`AzureAd:ClientId`**  
  The application (client) ID registered in azure ad.

- **`AzureAd:Instance`**  
  The URL of the azure ad service used to authenticate.

- **`AzureAd:TenantId`**  
  The unique identifier for your azure ad tenant.

- **`Environment`**  
  The environment which the app is running on.

- **`Logging:ApplicationInsights:LogLevel:Default`**
  The default logging level for the service when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**
  The default logging level for Microsoft specific information when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:LogLevel:Default`**
  The default logging level for the service; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`NotifyTableStorage:ConnectionString`**  
  The connection string for the GOV.UK Notify related azure storage account table. This grants access to the storage account table, when managed identites are not being used.

- **`PdsApplicationInsights:InstrumentationKey`**  
  The key value for Application Insights resource for logging purposes.

- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.

- **`StorageCache:ConnectionString`**  
  The connection string for the redis cache resource. (Local development will not let you connect to deployed Azure based redis resources and so a local one must be spun up. TODO add steps for local redis container once verified).

- **`StorageCache:ItemLifetimeInMinutes`**  
  The amount of time the data will be kept in redis before needing to be refreshed/recached.