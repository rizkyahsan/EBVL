# EBVL

EBVL adalah solution .NET 10 berbasis golden reference SolTem2-base. Identitas solution, namespace, assembly, schema aplikasi, permission scope, audience, dan judul aplikasi menggunakan prefix `EBVL`.

## Struktur

- `src/01.Shared`: enums, statics, dan DTO lintas layer.
- `src/02.BackEnd`: Domain, Services, Infrastructure, CQRS Logics, dan ASP.NET Core Minimal API.
- `src/03.FrontEnd`: Services, Infrastructure, CQRS Logics, dan Blazor Web App.
- `packages/nuget-offline`: paket privat `Pertamina.*` versi 1.0.4, termasuk dependency privat transitif.

Dependency graph dan pola Clean Architecture mengikuti golden reference. Infrastruktur yang dipertahankan mencakup EF Core, MediatR, autentikasi IdAMan/local identity, Serilog, OpenTelemetry/Azure Monitor, health checks, dan Hangfire.

## Prasyarat

- .NET SDK 10.0 yang mendukung target `net10.0`.
- Akses internet ke `nuget.org` untuk paket publik. Restore tidak mengakses feed TFS Pertamina.
- SQL Server hanya diperlukan untuk menjalankan aplikasi, bukan untuk restore/build.

## Restore dan build

Jalankan dari root repository:

```bash
dotnet restore EBVL.slnx
dotnet build EBVL.slnx --no-restore -m:1
```

Alternatif eksplisit untuk macOS:

```bash
./scripts/restore-macos.sh
dotnet build EBVL.slnx --no-restore -m:1
```

## Konfigurasi lokal

File secret aktual diabaikan Git. Buat konfigurasi lokal dari placeholder:

```bash
cp src/02.BackEnd/05.BackEnd.WebApi/secrets.example.json src/02.BackEnd/05.BackEnd.WebApi/secrets.json
cp src/03.FrontEnd/05.FrontEnd.WebUi/secrets.example.json src/03.FrontEnd/05.FrontEnd.WebUi/secrets.json
```

Ganti setiap nilai `__SET_LOCALLY__` hanya pada `secrets.json`. Jangan commit file tersebut. Sesuaikan endpoint contoh pada `appsettings.json` untuk environment tujuan.

## Menjalankan aplikasi

Setelah SQL Server dan konfigurasi lokal tersedia:

```bash
./scripts/run-all.sh
```

Profil default menjalankan Web API pada `https://localhost:44421/ebvl_api` dan Web UI pada `https://localhost:44422/ebvl`.

## Docker

Dockerfile berada pada:

- `src/02.BackEnd/05.BackEnd.WebApi/Dockerfile`
- `src/03.FrontEnd/05.FrontEnd.WebUi/Dockerfile`

Semua credential wajib diberikan saat runtime melalui secret manager atau environment aman; tidak ada credential produksi di repository.
