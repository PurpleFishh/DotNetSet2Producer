using Microsoft.Extensions.Configuration;
using Generator.Generators.ValueGenerator;
using Generator.Random;
using Producer.Config;
using Producer.Controller;
using Producer.Entity;
using Producer.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

AppConfig.Initialize(configuration);

var rnd = new DefaultRandomSource();
var carId = new CarIdGenerator().Next(rnd);

var schemas = Enum.GetValues<DataSchemas>().ToList();
var schemaIndex = rnd.NextInt(0, schemas.Count - 1);


var options = AppConfig.Current!;
var compression = Enum.Parse<CompressionKind>(options.Io.Compression, ignoreCase: true);
var writer = new FileSystemService(
    baseFolder: options.Paths.BaseFolder,
    vehicleId: carId,
    version: schemas[schemaIndex].ToString(),
    compress: compression,
    maxRecords: options.RotationPolicy.MaxRecords,
    maxBytes: options.RotationPolicy.MaxBytes,
    maxAge: TimeSpan.FromSeconds(options.RotationPolicy.TimeWindowSeconds),
    backlogThreshold: options.WindowSize.BackpressureMaxUnprocessed,
    backoffBase: TimeSpan.FromSeconds(options.WindowSize.BackoffBaseSeconds),
    backoffMax: TimeSpan.FromSeconds(options.WindowSize.BackoffMaxSeconds)
);


var controller = new CarController(carId, schemas[schemaIndex], writer);
var cts = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

AppDomain.CurrentDomain.ProcessExit += (s, e) => { writer.FinalizeOnShutdownSync(); };

try
{
    while (!cts.IsCancellationRequested)
    {
        await controller.InfoPublish();
        await Task.Delay(0, cts.Token);
    }
}
catch (OperationCanceledException)
{
}