using ERP.Application;
using ERP.Infrastructure;
using ERP.Infrastructure.Identity;
using ERP.Infrastructure.Persistence;
using ERP.WebUI.Hubs;
using ERP.WebUI.Services;
using JasperFx.CodeGeneration.Model;
using Microsoft.AspNetCore.Identity;
using Radzen;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================================================================================
//                                                          SERVICES
// ==================================================================================================================================

// --- Entity Framework & Identity ---
// DbContext registration moved to Infrastructure.DependencyInjection for consistency across WebUI/WebAPI
// (shared scoped instance for UnitOfWork + repositories)

builder.Services.AddIdentity < IdentityUser < Guid >, IdentityRole < Guid >>
    (
    options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;

        // User settings
        options.User.RequireUniqueEmail = false;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";
    }
    )

.AddEntityFrameworkStores < AppDbContext > ()
.AddDefaultTokenProviders();                // enables forgot password tokens which i don't know how they help us

// Configure cookie settings
builder.Services.ConfigureApplicationCookie
    (
    options =>
    {
        options.Cookie.HttpOnly = true;                             // javascript can not read the cookies (prevents XSS attacks)
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;   // HTTPS only (prevents man in the middle attacks)
        options.Cookie.SameSite = SameSiteMode.Strict;            // send this cookie IF the request came from MY website (Prevents CSRF)
        options.ExpireTimeSpan = TimeSpan.FromHours(1);          // Session length
        options.SlidingExpiration = true;                       // Extends session on activity
        options.LoginPath = "/Account/Login";                  // why are these in the cookies section?
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    }
    );

// --- Application & Infrastructure ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- Wolverine ---
// Host-level configuration: delegates to Application layer, then adds
// the EF Core service-location override. EF Core registers DbContextOptions<T>
// as an opaque lambda factory; Wolverine cannot inline this, so service location
// is required. This lives in the host because it is infrastructure-specific.
builder.Host.UseWolverine(options =>
{
    ERP.Application.DependencyInjection.ConfigureWolverine(options);

    // Allow service location for EF Core lambda factories (DbContextOptions<T>).
    // Wolverine 6 defaults to NotAllowed, which breaks EF Core's opaque registrations.
    // AllowedButWarn keeps the 5.x behavior; use AlwaysAllowed to silence the log warning.
    options.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;
});

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();  // UI interactions happen on the server via SignalR (real-time connection)
builder.Services.AddScoped < Radzen.NotificationService > ();
builder.Services.AddRadzenComponents();

// --- Current User Service (Blazor-specific) ---
builder.Services.AddScoped < ERP.Application.Common.Interfaces.ICurrentUserService, BlazorCurrentUserService > ();

builder.Services.AddSignalR();

var app = builder.Build();

// ==================================================================================================================================
//                                                          MIDDLEWARE PIPELINE
// ==================================================================================================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();          // If someone comes in via HTTP, kick them to HTTPS
app.UseStaticFiles();              // Serve CSS, JS, images from wwwroot folder
app.UseRouting();                 // Figure out which URL they're asking for

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents < ERP.WebUI.Components.App > ()
    .AddInteractiveServerRenderMode();                  // this adds the signalR stuff

app.MapHub < NotificationHub > ("/notificationHub");

// Seed default roles and admin user for the first time (if not available)
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await IdentitySeeder.SeedAsync(serviceProvider);
    // When the scope ends, everything is cleaned up (whatever this means)
}

app.Run();