global using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();
var app = builder.Build();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseOpenApi(); 
app.UseSwaggerUi3(s => s.ConfigureDefaults());
app.Run();