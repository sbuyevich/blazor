using Microsoft.EntityFrameworkCore;
using student_projects.Components;
using student_projects.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=student-projects.db";
var seedOptions = builder.Configuration.GetSection(DatabaseSeedOptions.SectionName).Get<DatabaseSeedOptions>() ?? new DatabaseSeedOptions();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
        .UseSeeding((context, _) => ApplicationDbContextSeed.Seed(context, seedOptions))
        .UseAsyncSeeding((context, _, cancellationToken) => ApplicationDbContextSeed.SeedAsync(context, seedOptions, cancellationToken)));

var app = builder.Build();

await ApplicationDbContextInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
