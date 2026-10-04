var builder = WebApplication.CreateBuilder(args);

// ── 1. Localization Services ────────────────────────────────────────────────
// Resource files live under /Resources/ and are named SharedResource.{culture}.resx
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// ── 2. MVC + View/DataAnnotations Localization ─────────────────────────────
var mvcBuilder = builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(Inventory_Management_System.SharedResource));
    });

// Enable Razor runtime compilation in Development so .cshtml changes
// are picked up without a full server restart.
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Inventory Module Services
builder.Services.AddScoped<Inventory_Management_System.Services.ICategoryService, Inventory_Management_System.Services.CategoryService>();
builder.Services.AddScoped<Inventory_Management_System.Services.IProductService, Inventory_Management_System.Services.ProductService>();
builder.Services.AddScoped<Inventory_Management_System.Services.ISupplierService, Inventory_Management_System.Services.SupplierService>();
builder.Services.AddScoped<Inventory_Management_System.Services.ISupplierProductService, Inventory_Management_System.Services.SupplierProductService>();
builder.Services.AddHttpClient<Inventory_Management_System.Services.IAIService, Inventory_Management_System.Services.AIService>();

// ── 3. RequestLocalizationOptions ──────────────────────────────────────────
var supportedCultures = new[] { "en", "ar", "fr", "es", "it", "de" };
var locOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Provider priority: Cookie (persisted preference) → QueryString → Accept-Language header
locOptions.RequestCultureProviders.Clear();
locOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());
locOptions.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
locOptions.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ── 4. Localization Middleware (must come BEFORE routing) ───────────────────
app.UseRequestLocalization(locOptions);

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Analytics}/{id?}")
    .WithStaticAssets();

app.Run();
