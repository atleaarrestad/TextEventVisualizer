using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TextEventVisualizer.Data;
using TextEventVisualizer.Repositories;
using TextEventVisualizer.Services;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=database.db");
    options.EnableServiceProviderCaching(false);
});


//repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

//services
builder.Services.AddHttpClient();
builder.Services.AddTransient<IHuggingFaceService, HuggingFaceService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IJsonService, JsonService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();

var app = builder.Build();


// Database migrations and initial creation
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred while applying migrations: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
