using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Αλλαγή σε false για εύκολη εγγραφή του admin
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4; // Μειώνουμε το μήκος για να επιτρέψουμε το "1234"
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<FuneralOfficeSystem.Services.PdfService>();

builder.Services.AddRazorPages();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = false;
});

var app = builder.Build();

// Αυτόματη εφαρμογή των migrations και δημιουργία της βάσης δεδομένων
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Αυτό θα δημιουργήσει τη βάση δεδομένων εάν δεν υπάρχει και θα εφαρμόσει όλα τα migrations
        context.Database.Migrate();
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Η βάση δεδομένων δημιουργήθηκε ή ενημερώθηκε επιτυχώς με τα migrations");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Σφάλμα κατά τη δημιουργία ή ενημέρωση της βάσης δεδομένων");
        throw;
    }
}

// Κλήση της SeedData για αρχικοποίηση δεδομένων
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        FuneralOfficeSystem.Data.SeedData.Initialize(serviceProvider).Wait();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Η αρχικοποίηση των δεδομένων από το SeedData ολοκληρώθηκε επιτυχώς");
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Σφάλμα κατά την αρχικοποίηση των δεδομένων από το SeedData");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();