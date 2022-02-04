using Microsoft.AspNetCore.HttpOverrides;
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

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

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
});

app.Use(async (context, next) =>
{
    // Security Headers
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' ajax.cloudflare.com static.cloudflareinsights.com");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer-when-downgrade");

    await next();
});

app.Run();