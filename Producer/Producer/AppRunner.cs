using Generator.Generators.ValueGenerator;
using Generator.Random;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Producer.Common;
using Producer.Infrastructure;

namespace Producer;

public class AppRunner(ILogger<AppRunner> log, IServiceProvider root)
{
    public async Task RunAsync()
    {
        var rnd = new DefaultRandomSource();
        var carId = new CarIdGenerator().Next(rnd);

        var schemas = Enum.GetValues<DataSchemas>().ToList();
        var schemaIndex = rnd.NextInt(0, schemas.Count - 1);

        using var vehicleScope = root.CreateVehicleScope(carId, schemas[schemaIndex]);
        var controller = vehicleScope.ServiceProvider.GetRequiredService<CarOperations>();

        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        AppDomain.CurrentDomain.ProcessExit += (s, e) => { controller.FinalizeWriting(); };

        try
        {
            while (!cts.IsCancellationRequested)
            {
                await controller.PublishCarData();
                await Task.Delay(0, cts.Token);
            }
        }
        catch (OperationCanceledException e)
        {
            log.LogError("Cancellation failed! {Exception}", e);
        }
    }
}