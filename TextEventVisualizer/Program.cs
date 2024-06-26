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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=database.db");
    options.EnableServiceProviderCaching(false);
});


//repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ITimelineRepository, TimelineRepository>();

//services
builder.Services.AddHttpClient();
builder.Services.AddTransient<ILargeLanguageModelService, LargeLanguageModelService>();
builder.Services.AddTransient<IHuggingFaceService, HuggingFaceService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IJsonService, JsonService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddSingleton<ITimelineService, TimelineService>();

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


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
