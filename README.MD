# Repro for https://github.com/dotnet/aspnet-api-versioning/issues/828

## Goal of this code

Support Versioned APIs  
* `[ApiVersion(...)]` is present on the method
* `api-version` is optionally specified on the request: when omitted, the first implemented/supported version must be assumed

Support Unversioned APIs:  
* `[ApiVersion(...)]` is NOT present on the method: this is a large migrated code-base where it's not present on certain methods and adding it would be a big effort
* api-version is never specified on the request
    
This is implemented by:
* DefaultApiVersion is set to 1.0
* Custom code for `app.UseSwagger` to make the api-version attribute optional in SwaggerUI
* `CustomApiVersionSelector` to selects the ApiVersion when it was missing on the request

## The problem
WeatherForecastController has three methods.  
Two are *Versioned APIs*: they have `[ApiVersion(2.0)]` specified. These work [fine](https://localhost:7227/WeatherForecast/today).  

The third method (`GetWeatherTomorrow`) is an *Unversioned API*: it has NO ApiVersion-attribute specified. This does [not work](https://localhost:7227/WeatherForecast/tomorrow).

When GetWeatherTomorrow is invoked, CustomVersionSelector.SelectVersion is passed an ApiVersionModel with model.Implemented and model.Support both containing '2.0'. This version was not specified on this method. **Where is this coming from?**  
Invoking it with api-version 2.0, 1.0 or no version at all does not work.

