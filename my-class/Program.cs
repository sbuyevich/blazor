using MyClass.Components;
using MyClass.Data;
using Microsoft.EntityFrameworkCore;
using MyClass.Options;
using MyClass.Services.Auth;
using MyClass.Services.BrowserStorage;
using MyClass.Services.ClassContext;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<Teacher>(builder.Configuration.GetSection("Teacher"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginStateService, LoginStateService>();
builder.Services.AddScoped<IClassContextService, ClassContextService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddSingleton<IPasswordHashService, PasswordHashService>();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
