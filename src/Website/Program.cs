using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Website.Common;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var services = builder.Services;
var config = builder.Configuration;

// Azure Key Vault
if (!string.IsNullOrEmpty(config["AzureKeyVault:Endpoint"]))
{
    try
    {
        var secretClient = new SecretClient(new Uri(config["AzureKeyVault:Endpoint"]!), new DefaultAzureCredential());
        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

// Application Insights
if (!string.IsNullOrEmpty(config["ApplicationInsights:ConnectionString"]))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = config["ApplicationInsights:ConnectionString"]!;
    });
}

// Database
services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(config.GetConnectionString("WebsiteDatabase")));

// Allows us to use controllers alongside razor pages
services.AddMvc();

// Bundle and minify our JS and CSS
services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyCssFiles();
    pipeline.MinifyJsFiles();

    pipeline.AddCssBundle("/css/bundle.min.css", "dist/purecss/*.css", "css/site.css");
    pipeline.AddJavaScriptBundle("/js/bundle.min.js", "dist/*.js");
});

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

app.Use(async (context, next) =>
{
    // Security Headers
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' ajax.cloudflare.com static.cloudflareinsights.com gist.github.com; style-src 'self' 'unsafe-inline' github.githubassets.com; frame-src 'self' www.youtube-nocookie.com; img-src 'self' i.ytimg.com;");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseWebOptimizer();

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.MapControllers();

// Run our migrations on start up
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.Run();