var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

// Add Session services.
builder.Services.AddSession(options =>
{
    // Set Session timeout to 30 minutes.
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Make the Session cookie essential.
    options.Cookie.IsEssential = true;
});

// Add Response Caching services.
builder.Services.AddResponseCaching();

// Add IMemoryCache services.
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure error handling.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP to HTTPS.
app.UseHttpsRedirection();

// Enable static files.
app.UseStaticFiles();

// Enable routing.
app.UseRouting();

// Enable response caching.
app.UseResponseCaching();

// Enable Session.
app.UseSession();

// Enable authorization.
app.UseAuthorization();

// Set Doctor/Index as the default page.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Doctor}/{action=Index}/{id?}");

app.Run();