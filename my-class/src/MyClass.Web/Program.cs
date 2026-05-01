using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using MyClass.Core.Data;
using MyClass.Core.Models;
using MyClass.Core.Options;
using MyClass.Core.Services;
using MyClass.Web.Components;
using MyClass.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();
builder.Services.AddMudServices();

builder.Services.AddDataProtection() // do i need it?
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")))
    .SetApplicationName("MyClass");

// database
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// options
builder.Services.Configure<TeacherOptions>(builder.Configuration.GetSection("Teacher"));
builder.Services.Configure<QuizOptions>(builder.Configuration.GetSection("Quiz"));

// services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginStateService, LoginStateService>();
builder.Services.AddScoped<IClassContextService, ClassContextService>();
builder.Services.AddScoped<IClassContextState, ClassContextState>();
builder.Services.AddScoped<IQuizContentService, QuizContentService>();
builder.Services.AddScoped<IQuizNotificationService, SignalRQuizNotificationService>();
builder.Services.AddScoped<IQuizSessionService, QuizSessionService>();
builder.Services.AddScoped<IQuizAnswerService, QuizAnswerService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISchoolClassService, SchoolClassService>();
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
app.MapHub<QuizHub>(QuizHub.Route);
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
