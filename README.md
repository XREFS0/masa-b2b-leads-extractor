# MASA B2B Leads Extractor

A Windows desktop application for discovering and exporting B2B leads (business name, address, phone, website, email) from online maps and search engines.

## Features

- Search businesses by category and location (country / state / city / zip code)
- Collect leads from Bing Maps search results
- Email mining from business websites (with Startpage fallback search)
- CSV and Excel (XLS) export with configurable columns
- Proxy support: no proxy, single proxy, random proxy list, free proxy lists, VPN
- Configurable request delay between searches
- Localized UI (English, Italian, German, French, Spanish)
- Dark theme

## Requirements

- Windows
- .NET Framework 4.8
- MySQL database (with geographic data: countries, regions, cities, zip codes)
- Visual Studio 2019/2022 or MSBuild (to build from source)

## Build

```
msbuild MasaB2BExtractor.sln /p:Configuration=Release /t:Build /nologo /v:m
```

Or open `MasaB2BExtractor.sln` in Visual Studio and build the `Release` configuration.

The output is written to `MasaB2BExtractor\bin\Release\`. The `lib\` folder (third-party assemblies) and `languages\` folder are copied to the output automatically.

## Database setup

The application needs a MySQL database containing geographic data (countries, regions, cities, zip codes) to build search tasks. The database connection is stored in an encrypted config file (`db32.dll`) that is read from the application folder. Create this file by building a `ConnectionSettings` object, serializing it to XML, and encrypting it with `SecurityHandler.Encrypt` using the same key used at runtime (see `ConnectionSettings.LoadAndDecript`).

The `DatabaseImporter` class contains helpers to import country/city data and zip codes into MySQL.

## Disclaimer

This tool performs automated requests against third-party websites. Before using it in production, review the Terms of Service of the target websites and applicable data-protection regulations (e.g. GDPR). Use responsibly and only on data you are allowed to collect.

## License

MIT ΓÇö see [LICENSE](LICENSE). Copyright (c) 2026 XREFS0.

