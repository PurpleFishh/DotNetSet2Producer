namespace Producer.Infrastructure.Config;

public sealed class PathOptions
{
    public string BaseFolder { get; set; } = "inbox";
    public bool PerVehicleSubfolder { get; set; } = true;
}

public sealed class RotationPolicyOptions
{
    public int MaxRecords { get; set; } = 5000;
    public long MaxBytes { get; set; } = 10 * 1024 * 1024;
    public int TimeWindowSeconds { get; set; } = 60;
}

public sealed class IoOptions
{
    public int BufferSizeBytes { get; set; } = 64 * 1024;
    public string Compression { get; set; } = "None";
}

public sealed class WindowSizeOptions
{
    public int BackpressureMaxUnprocessed { get; set; } = 200;
    public double BackoffBaseSeconds { get; set; } = 5;
    public double BackoffMaxSeconds { get; set; } = 120;
    public int ExponentCap { get; set; } = 8;
}

public enum FaultPhase
{
    BeforeHash,
    AfterHash
}

public sealed class FaultInjectionOptions
{
    public bool Enabled { get; set; } = false;
    public double DropRecordProb { get; set; } = 0.0;
    public double CorruptTailProb { get; set; } = 0.0;
    public int CorruptTailBytes { get; set; } = 100;
    public double TruncateProb { get; set; } = 0.0;
    public int TruncateTailBytes { get; set; } = 500;
}

public sealed class ProducerOptions
{
    public PathOptions Paths { get; set; } = new();
    public RotationPolicyOptions RotationPolicy { get; set; } = new();
    public IoOptions Io { get; set; } = new();
    public WindowSizeOptions WindowSize { get; set; } = new();
    public FaultInjectionOptions FaultInjection { get; set; } = new();
}