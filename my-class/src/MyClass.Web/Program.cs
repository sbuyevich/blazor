using MyClass.Components;
using MyClass.Core.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MyClass.Core.Options;
using MyClass.Core.Services.Auth;
using MyClass.Core.Services.BrowserStorage;
using MyClass.Core.Services.ClassContext;
using MyClass.Core.Services.Quiz;
using MyClass.Core.Services.Students;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")))
    .SetApplicationName("MyClass");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<TeacherOptions>(builder.Configuration.GetSection("Teacher"));
builder.Services.Configure<QuizOptions>(builder.Configuration.GetSection("Quiz"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginStateService, LoginStateService>();
builder.Services.AddScoped<IClassContextService, ClassContextService>();
builder.Services.AddScoped<IQuizContentService, QuizContentService>();
builder.Services.AddScoped<IQuizSessionService, QuizSessionService>();
builder.Services.AddScoped<IQuizAnswerService, QuizAnswerService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISessionStorageService, SessionStorageService>();
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
