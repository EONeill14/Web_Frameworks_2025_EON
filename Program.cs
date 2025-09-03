using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Hubs;
using Web_Frameworks_2025_EON.Models;
using Web_Frameworks_2025_EON.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure the Stripe API key
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// 1. Get the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Add the DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Add Identity services for BOTH Users and Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// 4. Add External Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection =
            builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    });

// 5. Add Application Services
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map Endpoints
app.MapHub<ChatHub>("/chatHub");
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await context.Database.MigrateAsync();

    // Seed Roles
    string[] roleNames = { "Admin", "RegisteredUser" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed Admin User
    var adminEmail = "admin@admin.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, "Pass123!!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Seed Sample Users and Items if the database is empty
    if (!context.Items.Any())
    {
        var user1Email = "user1@test.com";
        var user1 = await userManager.FindByEmailAsync(user1Email);
        if (user1 == null)
        {
            user1 = new ApplicationUser { UserName = user1Email, Email = user1Email, EmailConfirmed = true };
            await userManager.CreateAsync(user1, "Pass123!");
        }

        var user2Email = "user2@test.com";
        var user2 = await userManager.FindByEmailAsync(user2Email);
        if (user2 == null)
        {
            user2 = new ApplicationUser { UserName = user2Email, Email = user2Email, EmailConfirmed = true };
            await userManager.CreateAsync(user2, "Pass123!");
        }

        var user3Email = "user3@test.com";
        var user3 = await userManager.FindByEmailAsync(user3Email);
        if (user3 == null)
        {
            user3 = new ApplicationUser { UserName = user3Email, Email = user3Email, EmailConfirmed = true };
            await userManager.CreateAsync(user3, "Pass123!");
        }

        // Use a single, static date for all seeded items
        var seedDate = DateTime.UtcNow;

        context.Items.AddRange(
            new Item { Name = "Callaway Golf Driver", Description = "Men's driver, regular flex shaft.", ListingType = "Good", Brand = "Callaway", Condition = "Used", DatePosted = seedDate, IsApproved = true, Quantity = 1, CategoryId = 3, Owner = user1 },
            new Item { Name = "Beginner Golf Lesson", Description = "One-hour golf lesson.", ListingType = "Service", DatePosted = seedDate, IsApproved = true, Quantity = 5, CategoryId = 6, Owner = user1 },
            new Item { Name = "Head Tennis Racket", Description = "Adult-sized racket.", ListingType = "Good", Brand = "Head", Condition = "Like New", DatePosted = seedDate, IsApproved = true, Quantity = 1, CategoryId = 2, Owner = user2 },
            new Item { Name = "Mycro GAA Helmet", Description = "Senior helmet, green and white.", ListingType = "Good", Brand = "Mycro", Condition = "Used", DatePosted = seedDate, IsApproved = true, Quantity = 1, CategoryId = 1, Owner = user2 },
            new Item { Name = "Road Bicycle", Description = "Lightweight racing bike.", ListingType = "Good", Brand = "Giant", Condition = "Like New", DatePosted = seedDate, IsApproved = true, Quantity = 1, CategoryId = 4, Owner = user3 },
            new Item { Name = "Adidas Football Boots", Description = "Size 9, good condition.", ListingType = "Good", Brand = "Adidas", Condition = "Used", DatePosted = seedDate, IsApproved = true, Quantity = 1, CategoryId = 5, Owner = user3 }
        );

        await context.SaveChangesAsync();
    }
}

app.Run();