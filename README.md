# Azure API Management and Azure Functions Integration Defects

## Summary
We are attempting to take IP currently held in over 200 Nuget Packages and turn them into a set of RESTful APIs to enable easier consumption.

Our proposed architecture is based on creating "nano-services" using Azure Functions. Each Azure Function App would host a single HttpTrigger exposing a single OpenAPI Operation. The OpenAPI definitions from these "nano-services" would then be enlisted into Azure API Management, and composed into a series of APIs / products. APIM would also manage cross functional concerns such as authentication, caching, versioning etc. It should also expose a single, merged OpenAPI definition that will allow us to use [AutoRest](https://github.com/Azure/autorest) to generate a .NET / Node Client SDK. See the following architecture diagram for more information:

![Proposed Architecture combining Azure Functions and Azure API Management](https://github.com/endjin/AzureApimAndFunctionsIntegrationDefects/raw/master/Artefacts/Assets/Images/00-Target-Architecture.png "Proposed Architecture combining Azure Functions and Azure API Management")
