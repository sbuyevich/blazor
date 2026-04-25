using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using student_projects.Auth;
using student_projects.Components;
using student_projects.Data;
using student_projects.Models;
using student_projects.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=student-projects.db";
var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, ".keys");
var seedOptions = builder.Configuration.GetSection(DatabaseSeedOptions.SectionName).Get<DatabaseSeedOptions>() ?? new DatabaseSeedOptions();
var superUserOptions = builder.Configuration.GetSection(SuperUserOptions.SectionName).Get<SuperUserOptions>() ?? new SuperUserOptions();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSupplyValueFromFormProvider();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthenticationConstants.AdminPolicy, policy =>
        policy.RequireRole(AuthenticationConstants.AdminRole));
});
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("student-projects");
builder.Services.AddAuthentication(AuthenticationConstants.Scheme)
    .AddCookie(AuthenticationConstants.Scheme, options =>
    {
        options.LoginPath = AuthenticationConstants.LoginPath;
        options.LogoutPath = AuthenticationConstants.LogoutPath;
        options.AccessDeniedPath = AuthenticationConstants.LoginPath;
        options.Cookie.Name = AuthenticationConstants.CookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(seedOptions);
builder.Services.AddSingleton(superUserOptions);
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<Student>, PasswordHasher<Student>>();

var app = builder.Build();

await ApplicationDbContextInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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
