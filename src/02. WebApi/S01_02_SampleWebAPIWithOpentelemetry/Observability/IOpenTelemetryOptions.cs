﻿namespace S01_02_SampleWebAPIWithOpentelemetry;

public interface IOpenTelemetryOptions
{
    string AzureMonitorConnectionString { get; }

    bool EnableTraces { get; }
    bool EnableMetrics { get; }

    double TracingSamplingRatio { get; }

    IEnumerable<string> ActivitySources { get; }
    IEnumerable<string> Meters { get; }

    IEnumerable<string> ExcludedHttpHosts { get; }

    IEnumerable<string> DurationMetricTags { get; }
}
