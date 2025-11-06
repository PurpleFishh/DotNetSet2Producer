namespace Producer.Business.Services.Implementation.Backpressure;

public class BackpressureOptions
{
    public int BacklogThreshold { get; set; } = 200;
    public TimeSpan BackoffBase { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan BackoffMax { get; set; } = TimeSpan.FromSeconds(120);
}