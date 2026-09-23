var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Inventory Module Services
builder.Services.AddScoped<Inventory_Management_System.Services.ICategoryService, Inventory_Management_System.Services.CategoryService>();
builder.Services.AddScoped<Inventory_Management_System.Services.IProductService, Inventory_Management_System.Services.ProductService>();
builder.Services.AddScoped<Inventory_Management_System.Services.ISupplierService, Inventory_Management_System.Services.SupplierService>();
builder.Services.AddScoped<Inventory_Management_System.Services.ISupplierProductService, Inventory_Management_System.Services.SupplierProductService>();
// Register AI Service
builder.Services.AddHttpClient<Inventory_Management_System.Services.IAIService, Inventory_Management_System.Services.AIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Analytics}/{id?}")
    .WithStaticAssets();

app.Run();
