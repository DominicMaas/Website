using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Website.Common;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var services = builder.Services;

services.AddMvc();

services.AddSingleton<SoundByteAuthenticationService>();

var mvcBuilder = services.AddRazorPages(options =>
    options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer())));

if (environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

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