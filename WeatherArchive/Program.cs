using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using WeatherArchive.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WeatherArchiveContext>(options => 
                                                     options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherArchiveDb")))
                .AddControllersWithViews();

var app = builder.Build();

// Applying migrations.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeatherArchiveContext>();
    db.Database.Migrate();
}

var option = new RewriteOptions();
option.AddRedirect("^$", "WeatherArchive");

app.UseRewriter(option);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/WeatherArchive/Error");
    app.UseHsts();
}

app.UseHttpMethodOverride(new HttpMethodOverrideOptions()
{
    FormFieldName = 
        HtmlHelperExtensions.HttpMethodOverrideFormName
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WeatherArchive}/{action=Index}/{id?}");

app.Run();
