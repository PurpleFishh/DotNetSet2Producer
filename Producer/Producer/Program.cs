using System.CodeDom.Compiler;
using System.Text.Json;
using Generator;
using Generator.Generators.Helper;
using Generator.Generators.ValueGenerator;
using Generator.Random;
using Producer.CarDataGenerator;
using Producer.CarDataGenerator.Generators;
using Producer.Generator;

var fuelPct = FuelPctGenerator.Get();
var engineOn = EngineStatusGenerator.Get();
var speed = SpeedGenerator.Get();
var coolant = CoolantInfoGenerator.Get();
var gps = GpsGenerator.Get();
var odo = OdoInfoGenerator.Get();


var rnd = new DefaultRandomSource(seed: "V-042".GetHashCode());
var ctx = new GenerationContext();
var carId = new CarIdGenerator().Next(rnd, ctx);

var predefinedValues = new Dictionary<string, Func<object>>
{
    ["vehicleId"] = () => carId,
    ["tsUtc"] = () => DateTime.UtcNow
};

CarDtoV1 DtoMapper(IReadOnlyDictionary<string, object> d) => AutoMapper.MapTo<CarDtoV1>(d, predefinedValues);

var builder = new RecordBuilder<CarDtoV1>(DtoMapper)
    .AddStep("engineOn", engineOn)
    .AddStep("speedKmh", new Round<double>(speed, 1))
    .AddStep("fuelPct", new Round<double>(fuelPct, 1))
    .AddStep("coolantTempC", new Round<double>(coolant, 1))
    .AddStep("gps", gps)
    .AddStep("odoKm", odo);

for (var i = 0; i < 10; i++)
{
    var rec = builder.Build(rnd, ctx);
    Console.WriteLine(JsonSerializer.Serialize(rec, new JsonSerializerOptions { WriteIndented = true }));
}