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

These two functions have identical OpenAPI operations other than they are named Operation A and Operation B and have distinct (but identical) request / response models.

The source for FunctionAppA and FunctionAppB is included in this repo. Publish them to azure as endjinfunctionappa and endjinfunctionappb if you want to repro the steps below.

**Note** The sample projects use the [OpenAPI Converter for Visual Studio 2017](https://github.com/endjin/Endjin.OpenAPI.Converters/releases) to autogenerate the swagger.json file Azure Functions required from the more easy to work with swagger.yaml files.

## Expected Behaviour

The behaviour we expected was to be able to import the two deployed Function Apps via the APIM "API" Portal UI; for the first Function App using the "Create API" experience and selecting "Function App" and for the second Function App using the "Import API" feature. The expected behaviour was to merge the two OpenAPI definitions into a single document containing both sets of data and to automatically configure the Function Keys to automatically authenticate requests send to the imported Azure Functions.

## Defects

* Functions added via the UI do not automatically add the Function Key to the generated policy - thus all calls made via the APIM generated API or the Test UI fail with a 401 not authorised error.
* While the first Function is imported correctly (i.e. Operation Id as specified in the original OpenAPI definition) any subsequent Function is incorrectly imported; a different Operation Id is generated, model schemas are ignored. 
* Because subsequent OpenAPI documents are not been imported / merged correctly the Test UI is incorrect / fails to run.
* This also means that API definitions as listed in the public Developer Portal are also incorrect / fails to run.

## Repro Steps

* The source for FunctionAppA and FunctionAppB is included in this repo.
* From your APIM instance select "APIs" from the menu. 
* Click the "+ Add API" link
* Select "Function App"
* Click browse from the modal dialog and select EndjinFunctionAppA. Fill in the rest of the details as follows:

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/01-Create-From-Function-App.png "")

As you can see from the screenshot below, the OpenAPI definition has been correctly imported. The original Operation Id "Operation_A" and display name "Operation - A" have been used.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/02-Imported-Operation-A.png "")

* Select "Import" from the "Composite Functions" context menu.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/03-Import-FunctionAppB.png "")

* Select EndjinFunctionAppB.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/04-Import-FunctionAppB.png "")

**Defect 1** Expected behaviour: values displayed honour values in supplied OpenAPI definition. Actual: EndjinFunctionAppB's Operation Id has not been honoured / imported correctly. 

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/05-Imported-Operation-B.png "")

**Defect 1 Continued...** Looking at the Form View for Operation-B you can see that the *Display Name* is incorrect as is the *Name* as a guid has been used rather than the valid Operation Id supplied. It is also missing the supplied description.

[Expected](https://raw.githubusercontent.com/endjin/AzureApimAndFunctionsIntegrationDefects/master/Artefacts/Assets/OpenAPI/Merged-Expected.yaml) vs [Actual](https://raw.githubusercontent.com/endjin/AzureApimAndFunctionsIntegrationDefects/master/Artefacts/Assets/OpenAPI/Merged-Actual.yaml) example merged OpenAPI files have been added to this repo.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/05-Imported-Operation-B-Form-View.png "")

* Navigate to the test console for Operation - A
* Click the text button

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/06-Operation-A-Test-Console.png "")

**Defect 2** Imported Function App does not have Function Authorization token automatically set in Policy file. This means that calls to the external API / Test UI fail. Manually updating the policy file to add the authentication key enables calls to authenticate correctly. [Expected](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Policy/Expected.txt) vs [Actual](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Policy/Actual.txt) example policy files have been added to this repo.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/07-Operation-A-401.png "")

**Defect 3** Because EndjinFunctionAppB OpenAPI document has not been imported / merged correctly the test UI is incorrect.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/08-Operation-B.png "")

Navigating to the Development Portal, EndjinFunctionAppA - Operation - A API is correctly documented.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/09-Developer-Portal-Operation-A.png "")

**Defect 4** Because EndjinFunctionAppB OpenAPI document has not been imported / merged correctly the Developer Portal UI / documentation is incorrect.

![](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/10-Developer-Portal-Operation-B.png "")
