# Runtime Data Usage Batching

The runtime-data API accepts usage metrics in bounded batches to reduce signed HTTP request overhead.

Current server-side storage implementation uses the existing `TableStorageBase<UsageMetrics>` insert path for each record in the batch. If a provider-native batch insert is introduced in `TableStorageBase`, `UsageMetricsRepo.AddMetricsAsync` is the single seam that should adopt it.
