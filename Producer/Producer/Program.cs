using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using Producer.Business;
using Producer.Infrastructure;
using Producer.Presentation.Controller;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddVehicleContext();
        services.AddApplication();
        services.AddInfrastructure(context.Configuration);
        services.AddScoped<CarController>();
        services.AddTransient<AppRunner>();
    })
    .Build();

await host.Services.GetRequiredService<AppRunner>().RunAsync();