using FastEndpoints;
using FastEndpoints.Swagger;
using Sample.Infrastructure;
using Sample.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddFastEndpoints()
    .SwaggerDocument();

builder.AddNpgsqlDbContext<CoreDbContext>("core-db");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseFastEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
}


app.MapDefaultEndpoints();

app.Run();
