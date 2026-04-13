using Htmx.TagHelpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Website.Common;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var services = builder.Services;
var config = builder.Configuration;

// Data Protection
services.AddDataProtection().PersistKeysToDbContext<DatabaseContext>();

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

// Compression
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;

    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
});

// Razor pages (most pages on this site)
var razorBuilder = services.AddRazorPages(options =>
{
    options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer()));

    options.Conventions.AuthorizePage("/Admin");
});

// Allows us to use controllers alongside razor pages
var mvcBuilder = services.AddMvc();

// Allows compiling within development environment
if (environment.IsDevelopment())
{
    razorBuilder.AddRazorRuntimeCompilation();
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Bundle and minify our JS and CSS
services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyCssFiles();
    pipeline.MinifyJsFiles();

    pipeline.AddCssBundle("/css/bundle.min.css", "dist/purecss/*.css", "css/site.css");
    pipeline.AddJavaScriptBundle("/js/bundle.min.js", "dist/*.js");
});

// Basic Authentication
services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/signout";
    })
    .AddMicrosoftAccount(options =>
    {
        options.AuthorizationEndpoint = "https://login.microsoftonline.com/a9835ab3-a9f8-4566-8dbe-70562eb068e1/oauth2/v2.0/authorize";
        options.TokenEndpoint = "https://login.microsoftonline.com/a9835ab3-a9f8-4566-8dbe-70562eb068e1/oauth2/v2.0/token";

        options.ClientId = config["Authentication:Microsoft:ClientId"] ?? string.Empty;
        options.ClientSecret = config["Authentication:Microsoft:ClientSecret"] ?? string.Empty;
    });

services.AddAuthorization();

// Services
services.AddSingleton<SoundByteAuthenticationService>();
services.AddSingleton<R2>();

// ----- App ----- //

var app = builder.Build();

app.Use(async (context, next) =>
{
    // Security Headers
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' ajax.cloudflare.com static.cloudflareinsights.com gist.github.com; style-src 'self' 'unsafe-inline' github.githubassets.com; frame-src 'self' www.youtube-nocookie.com; img-src 'self' i.ytimg.com images.dominicmaas.co.nz;");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

var forwardOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
};

forwardOptions.KnownIPNetworks.Clear();
forwardOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardOptions);

if (!environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseResponseCompression();

app.UseWebOptimizer();

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHtmxAntiforgeryScript();

app.MapRazorPages();

app.MapControllers();

// Run our migrations on start up
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.Run();