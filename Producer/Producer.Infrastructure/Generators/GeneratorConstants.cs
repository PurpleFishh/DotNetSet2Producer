namespace Producer.Infrastructure.Generators;

public class GeneratorTimeOptions
{
    public TimeOnly MiddayTime { get; set; } = new(12, 0, 0);
    public TimeOnly DayStartTime { get; set; } = new(8, 0, 0);
}