using System.Text.Json;
using Generator.Generators.ValueGenerator;
using Generator.Random;
using Producer.Mappers;
using Producer.Services;

var rnd = new DefaultRandomSource();
var carId = new CarIdGenerator().Next(rnd);

var carGenerator = new CarTelemetryService(carId, null);

for (var i = 0; i < 10; i++)
{
    var rec = carGenerator.Next();
    var jsonSettings = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    Console.WriteLine(JsonSerializer.Serialize(rec.ToDtoV2(), jsonSettings));
}