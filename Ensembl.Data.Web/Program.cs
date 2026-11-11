using Ensembl.Data.Web.Configuration.Extensions;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddServices();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
