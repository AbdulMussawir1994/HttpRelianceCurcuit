using System.ComponentModel.DataAnnotations;

namespace OrderServiceAPI.ConfigurationSettings;

public class ResilienceSettings
{
    public const string SectionName = "Resilience";

    [Range(0, 10)]
    public int RetryCount { get; init; } = 2;

    [Range(0, 60000)]
    public int RetryDelayMilliseconds { get; init; } = 250;

    [Range(0.01, 1.0)]
    public double CircuitFailureRatio { get; init; } = 0.5;

    [Range(2, 10000)]
    public int CircuitMinimumThroughput { get; init; } = 5;

    [Range(1, 300)]
    public int CircuitSamplingSeconds { get; init; } = 30;

    [Range(1, 300)]
    public int CircuitBreakSeconds { get; init; } = 20;

    [Range(1, 120)]
    public int AttemptTimeoutSeconds { get; init; } = 3;
}
