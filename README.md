# Azure API Management & Functions Integration Defects

## Summary
We are attempting to take IP currently held in over 200 Nuget Packages and turn them into a set of RESTful APIs to enable easier consumption.

Our proposed architecture is based on creating "nano-services" using Azure Functions. Each Azure Function App would host a single HttpTrigger exposing a single OpenAPI Operation. The OpenAPI definitions from these "nano-services" would then be enlisted into Azure API Management, and composed into a series of APIs / products. APIM would also manage cross functional concerns such as authentication, caching, versioning etc. It should also expose a single, merged OpenAPI definition that will allow us to use [AutoRest](https://github.com/Azure/autorest) to generate a .NET / Node Client SDK. See the following architecture diagram for more information:

![Proposed Architecture combining Azure Functions and Azure API Management](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/00-Target-Architecture.png "Proposed Architecture combining Azure Functions and Azure API Management")

To explore this candidate architecture, we created a technical spike which consisted of two Azure Functions Apps, each containing a single HttpTrigger with simple request / response models. Each function also had a manually crafted OpenAPI document, validated using [editor.swagger.io](https://editor.swagger.io). These functions were deployed into Azure as:

* https://endjinfunctionappa.azurewebsites.net
* https://endjinfunctionappb.azurewebsites.net

Where their swagger documents are available from:

* https://endjinfunctionappa.azurewebsites.net/admin/host/swagger?code= &lt;Function Key&gt;
* https://endjinfunctionappb.azurewebsites.net/admin/host/swagger?code= &lt;Function Key&gt;

## Expected Behaviour

The behaviour we expected was to be able to import the two deployed Function Apps via the APIM "API" Portal UI; for the first Function App using the "Create API" experience and selecting "Function App" and for the second Function App using the "Import API" feature. The expected behaviour was to merge the two OpenAPI definitions into a single document containing both sets of data and to automatically configure the Function Keys to automatically authenticate requests send to the imported Azure Functions.

## Defects

1. Functions added via the UI do not automatically add the Function Key to the generated policy - thus all calls made via the Test UI fail with a 401 not authorised error.
2. While the first Function is imported correctly (i.e. Operation Id as specified in the original OpenAPI definition) any subsequent Function is incorrectly imported; a different Operation Id is generated, model schemas are ignored.

## Repro Steps

* From your APIM instance select "APIs" from the menu. 
* Click the "+ Add API" link
* Select "Function App"
* Click browse from the modal dialog and select EndjinFunctionAppA. Fill in the rest of the details as follows:

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/01-Create-From-Function-App.png "")

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/02-Imported-Operation-A.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/03-Import-FunctionAppB.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/04-Import-FunctionAppB.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/05-Imported-Operation-B.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/06-Operation-A-Test-Console.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/07-Operation-A-401.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/08-Operation-B.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/09-Developer-Portal-Operation-A.png "")
![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/10-Developer-Portal-Operation-B.png "")
