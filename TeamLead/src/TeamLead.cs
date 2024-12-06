using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TeamLead.di;
using TeamLead.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureLogging();
builder.Services.AddHttpClient<TeamLeadService>();
builder.Services.ConfigureServices();
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();