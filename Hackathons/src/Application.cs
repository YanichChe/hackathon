using Hackathon.di;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.ConfigureServices();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

using var host = builder.Build();
host.Run();
