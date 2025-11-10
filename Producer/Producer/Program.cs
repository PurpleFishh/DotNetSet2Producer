using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using Producer.Business;
using Producer.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddVehicleContext();
        services.AddApplication();
        services.AddInfrastructure(context.Configuration);
        services.AddScoped<CarOperations>();
        services.AddTransient<AppRunner>();
    })
    .Build();

await host.Services.GetRequiredService<AppRunner>().RunAsync();