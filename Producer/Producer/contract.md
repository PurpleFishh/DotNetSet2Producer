## Folders & filenames

* Folders: `inbox/` (producer writes), `archive/`, `error/`.
* Data files: `telemetry_YYYYMMDD_HHMMSS_<vehicleId>.jsonl[.gz]`
* Sidecar: same name + `.meta.json`
* Encoding: UTF-8 (no BOM), line endings `\n`.

## Atomicity & safety

* Producer writes to `*.tmp`, fsyncs, then **atomic rename** to final name (same volume).
* Consumer must ignore any file ending in `.tmp`.

## Sidecar metadata (required)

```json
{
  "version": "1.0",
  "createdUtc": "2025-10-23T12:00:30Z",
  "recordCount": 1200,
  "sha256": "<hex-lower>",
  "encoding": "utf8",
  "compression": "none|gzip"
}
```

* `sha256` is computed over the file **as stored on disk** (gzip bytes if compressed).

## Data format

* **JSON Lines**: one JSON object per line; no array wrappers.
* Timestamps are UTC (`tsUtc`).

## Schemas & versions

### v1 data schema (TelemetryRecordV1)

```json
{
  "vehicleId": "V-042",
  "tsUtc": "2025-10-23T12:00:05Z",
  "speedKmh": 72.3,
  "fuelPct": 57.2,
  "coolantTempC": 92.1
}
```

Validation (JSON Schema draft 2020-12):

* `vehicleId`: string, non-empty
* `tsUtc`: string, `date-time`
* `speedKmh`: number
* `fuelPct`: 0–100
* `coolantTempC`: number

### v2 data schema (TelemetryRecordV2, additive to v1)

```json
{
  "vehicleId": "V-042",
  "tsUtc": "2025-10-23T12:00:05Z",
  "speedKmh": 72.3,
  "fuelPct": 57.2,
  "coolantTempC": 92.1,
  "gps": { "lat": 52.5201, "lon": 13.4049 },
  "engineOn": true,
  "odoKm": 15432.6
}
```

Notes:

* New fields: `gps.lat`, `gps.lon`, `engineOn`, `odoKm`.
* Publish `telemetry.v2.schema.json`; consumers that only support v1 must reject `version` `2.x`.

## Compression

* Optional GZip. If enabled: file ends with `.jsonl.gz` and sidecar `compression` = `"gzip"`. Otherwise `"none"`.

## Rotation (recommendation)

Rotate files by size (≈5–20 MB) or time (≈30–60 s), targeting 500–5k records/file.

### Quick examples

**Filename pair**

```
inbox/telemetry_20251023_120000_V-042.jsonl
inbox/telemetry_20251023_120000_V-042.jsonl.meta.json
```

**One v1 line**

```json
{"vehicleId":"V-042","tsUtc":"2025-10-23T12:00:05Z","speedKmh":72.3,"fuelPct":57.2,"coolantTempC":92.1}
```

**One v2 line**

```json
{"vehicleId":"V-042","tsUtc":"2025-10-23T12:00:05Z","speedKmh":72.3,"fuelPct":57.2,"coolantTempC":92.1,"gps":{"lat":52.5201,"lon":13.4049},"engineOn":true,"odoKm":15432.6}
```
