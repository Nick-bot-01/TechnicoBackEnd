using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Services;
using TechnicoBackEnd.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//DB Context must exist on Creation before Controllers (DI) -> Dependency Injection
builder.Services.AddDbContext<TechnicoDbContext>();

builder.Services.AddScoped<IRepairService, RepairService>();
builder.Services.AddScoped<IRepairValidation, RepairValidation>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserValidation, UserValidation>();

builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IPropertyValidation, PropertyValidation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
