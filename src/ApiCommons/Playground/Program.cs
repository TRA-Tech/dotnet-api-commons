using ApiCommons.Middlewares.DbTransaction;
using Microsoft.EntityFrameworkCore;
using Playground.Entities;
using Playground.Middlewares;
using Playground.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NorthwindDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind"));
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<PlaygroundExceptionHandler>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.UseDbTransaction();

app.MapControllers();

app.Run();
