using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Services;
using TechnicoBackEnd.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
