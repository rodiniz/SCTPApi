global using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();
builder.Services.AddMemoryCache();
builder.Services.AddCors();
var app = builder.Build();
app.UseAuthorization();

app.UseOpenApi();
app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials()); // allow credentials
app.UseSwaggerUi3(s => s.ConfigureDefaults());
app.UseFastEndpoints();
app.Run();
