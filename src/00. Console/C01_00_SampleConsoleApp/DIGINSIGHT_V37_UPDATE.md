# Diginsight v3.7 Configuration Update

This workspace has been updated to conform to Diginsight v3.7 configuration schema.

## Key Changes Applied

### 1. Property Renames
- **`RecordSpanDurations`** → <mark>**`RecordSpanDuration`**</mark> (singular)
  - Location: C# code in `ObservabilityExtensions.cs` files
  - Example: `dao.RecordSpanDuration = true;`

### 2. Interface Changes
The following interfaces were renamed but may still show deprecation warnings in older code:
- **`IActivityLoggingSampler`** → <mark>**`IActivityLoggingFilter`**</mark>
- **`NameBasedActivityLoggingSampler`** → <mark>**`OptionsBasedActivityLoggingFilter`**</mark>
- **`IDiginsightActivitiesMetricOptions`** → <mark>**`IMetricRecordingOptions`**</mark>

### 3. Property Removals
- **`MetricUnit`** property has been removed (no longer used in configuration)

## Files Updated
- `02. WebApi/S01_01_SampleWebAPIWithStartupSequence/Observability/ObservabilityExtensions.cs`
  - Changed `RecordSpanDurations` to <mark>`RecordSpanDuration`</mark>

## Configuration Files
All `appsettings.json` files were reviewed and are already compatible with v3.7 schema as they don't use the deprecated property names.

## References
- [Diginsight v3.7 Release Changelog](https://diginsight.github.io/telemetry/src/docs/10.%20ChangeLog/20251112%20-%20changes%20for%20release%203.7.html)
- [Full Migration Guide](E:\dev.darioa.live\Diginsight\telemetry\src\docs\10. ChangeLog\20251112 - changes for release 3.7.md)

## Next Steps
If you encounter deprecation warnings or compilation errors:
1. Update service registrations to use new interface names
2. Replace deprecated property names in configuration
3. Remove any references to `MetricUnit` property
4. Test activity filtering and metric recording functionality

---
*Updated: [Current Date]*
*Diginsight Version: 3.7+*
