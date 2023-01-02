using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Website.Common;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var services = builder.Services;
var config = builder.Configuration;

// Azure Key Vault
if (!string.IsNullOrEmpty(config["AzureKeyVault:Endpoint"]))
{
    var secretClient = new SecretClient(new Uri(config["AzureKeyVault:Endpoint"]!), new DefaultAzureCredential());
    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
}

// Application Insights
if (!string.IsNullOrEmpty(config["ApplicationInsights:ConnectionString"]))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = config["ApplicationInsights:ConnectionString"]!;
    });
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

app.MapRazorPages();

app.MapControllers();

app.Use(async (context, next) =>
{
    // Security Headers
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' ajax.cloudflare.com static.cloudflareinsights.com gist.github.com; style-src 'self' 'unsafe-inline' github.githubassets.com; frame-src 'self' www.youtube-nocookie.com; img-src 'self' i.ytimg.com;");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

app.Run();