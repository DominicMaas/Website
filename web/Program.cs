using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Website.Common;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var services = builder.Services;
var config = builder.Configuration;

// Azure Key Vault
var azureClientId = config["AzureKeyVault:ClientId"];
var azureClientSercret = config["AzureKeyVault:ClientSecret"];

if (!string.IsNullOrEmpty(azureClientId) && !string.IsNullOrEmpty(azureClientSercret))
{
    config.AddAzureKeyVault(config["AzureKeyVault:Endpoint"], azureClientId, azureClientSercret, new DefaultKeyVaultSecretManager());
}

// Application Insights
if (!string.IsNullOrEmpty(config["ApplicationInsights:ConnectionString"]))
{
    builder.Services.AddApplicationInsightsTelemetry(config["ApplicationInsights:ConnectionString"]);
}

services.AddMvc();

// Services
services.AddSingleton<SoundByteAuthenticationService>();

var mvcBuilder = services.AddRazorPages(options =>
    options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer())));

if (environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// ----- App ----- //

var app = builder.Build();

if (!environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();

    endpoints.MapControllers();

    endpoints.MapControllerRoute("blog", "blog/{controller=Blog}/{action=Index}");
    endpoints.MapControllerRoute("blog-post", "blog/{id?}", new { controller = "Blog", action = "Post" });
});

app.Run();