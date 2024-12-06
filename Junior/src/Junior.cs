using Junior.di;
using Junior.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureLogging();
builder.Services.AddHttpClient<JuniorService>();
builder.Services.ConfigureServices();
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();