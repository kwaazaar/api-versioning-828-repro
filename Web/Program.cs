using api_versioning_828_repro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;

    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0); // Status is ignored by our CustomApiVersionSelector
    options.ApiVersionSelector = new CustomApiVersionSelector(options);
});
builder.Services.AddVersionedApiExplorer(options =>
{
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
    // note: the specified format code will format the version as "'v'major[.minor][-status]"
    //options.GroupNameFormat = "V-'vvv'"; // Fallback to x-unspecified when no ApiVersionAttribute is present
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger(options =>
{
    // We make the api-version parameter optional
    options.PreSerializeFilters.Add((doc, req) =>
    {
        var apiVersionParameterName = "api-version";
        var parameters = doc.Paths.SelectMany(path => path.Value.Parameters.Where(p => p.Name == apiVersionParameterName)) // Doc-level
            .Concat(doc.Paths.SelectMany(path => path.Value.Operations)
            .SelectMany(o => o.Value.Parameters.Where(p => p.Name == apiVersionParameterName))); // Operation-level

        foreach (var p in parameters)
        {
            p.Required = false; // Todo: skip if in path
        }

    });


    //// We remove the api-version parameter from all paths and operations, as it needs to be optional
    //options.PreSerializeFilters.Add((doc, req) =>
    //{
    //    var apiVersionParameterName = "api-version";

    //    // Remove completely
    //    foreach (var path in doc.Paths.Select(p => p.Value))
    //    {
    //        var apiVersionParameter = path.Parameters.FirstOrDefault(p => p.Name == apiVersionParameterName);
    //        if (apiVersionParameter != null)
    //        {
    //            path.Parameters.Remove(apiVersionParameter);
    //        }
    //    }

    //    foreach (var op in doc.Paths.SelectMany(path => path.Value.Operations).Select(o => o.Value))
    //    {
    //        var apiVersionParameter = op.Parameters.FirstOrDefault(p => p.Name == apiVersionParameterName);
    //        if (apiVersionParameter != null)
    //        {
    //            op.Parameters.Remove(apiVersionParameter);
    //        }
    //    }
    //});


    options.RouteTemplate = "swagger/{documentName}/docs.json";
});
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    options.RoutePrefix = "swagger";

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/docs.json", description.GroupName);
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
