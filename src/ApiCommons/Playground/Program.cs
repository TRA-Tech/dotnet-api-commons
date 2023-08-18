using ApiCommons.Extensions;
using ApiCommons.GeneralResponse;
using ApiCommons.Middlewares.GlobalErrorHandler;
using ApiCommons.Middlewares.DbTransaction;
using Microsoft.EntityFrameworkCore;
using Playground.Entities;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NorthwindDbContext>(options =>
{
    options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=northwind;Trusted_Connection=True;");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalErrorHandler(async (serviceProvider, context, exp) =>
{
    var contextResponse = context.Response;
    contextResponse.ContentType = "application/json";
    contextResponse.StatusCode = (int)HttpStatusCode.InternalServerError;

    var response = Response<object>.Fail(
        HttpStatusCode.InternalServerError,
        exp.Message,
        new
        {
            context.Request.Path,
        }
    );

    await contextResponse.WriteAsync(response.ToJson());
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDbTransaction(async (serviceProvider, context, exp) =>
{
    var contextResponse = context.Response;
    contextResponse.ContentType = "application/json";
    contextResponse.StatusCode = (int)HttpStatusCode.InternalServerError;

    var response = Response<object>.Fail(
        HttpStatusCode.InternalServerError,
        exp.Message,
        new
        {
            context.Request.Path,
        }
    );

    await contextResponse.WriteAsync(response.ToJson());
});

app.MapControllers();

app.Run();
