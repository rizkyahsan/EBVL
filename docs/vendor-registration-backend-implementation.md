# Vendor Registration Backend Implementation

Dokumen ini menjadi panduan implementasi backend dan integrasi FrontEnd untuk modul Vendor Registration. Implementasi dilakukan bertahap agar draft registrasi dan file tersimpan di backend, bukan hanya di state browser.

## 1. Domain Entities

Buat entity pada:

```text
src/02.BackEnd/01.BackEnd.Domain/Entities/
```

Entity yang diperlukan:

- `VendorRegistration`
- `VendorRegistrationBrand`
- `VendorRegistrationQuestionnaireAnswer`
- `VendorRegistrationDocument`
- `VendorRegistrationHistory`
- `VendorRegistrationSession`

`VendorRegistration` menjadi aggregate root. Entity lainnya menggunakan `VendorRegistrationId` sebagai foreign key.

Field utama `VendorRegistration`:

- Nomor SAP vendor
- Informasi perusahaan dan PIC
- Layanan perusahaan
- Negara dan alamat pabrik
- Status perusahaan
- Informasi representative
- Status registrasi
- Tahap registrasi terakhir
- Waktu submit dan verifikasi
- Informasi penolakan atau revisi
- Standard audit fields

Gunakan entity `FileStorage` yang sudah tersedia untuk menyimpan referensi binary file.

## 2. EF Core Configuration, DbSet, Seeder, and Migration

Buat konfigurasi Entity Framework pada:

```text
src/02.BackEnd/03.BackEnd.Infrastructure/Database/Configurations/
```

File yang diperlukan:

```text
VendorRegistrationConfiguration.cs
VendorRegistrationBrandConfiguration.cs
VendorRegistrationQuestionnaireAnswerConfiguration.cs
VendorRegistrationDocumentConfiguration.cs
VendorRegistrationHistoryConfiguration.cs
VendorRegistrationSessionConfiguration.cs
```

Konfigurasi mencakup:

- Table name dan schema `EBVL`
- Column type dan maximum length
- Primary key
- Foreign key
- Delete behavior
- Unique dan filtered index
- Relasi aggregate

Tambahkan `DbSet` pada:

```text
DatabaseService.DbSets.cs
IDatabaseService.DbSet.cs
```

Seeder hanya diperlukan jika definisi pertanyaan questionnaire atau document type disimpan sebagai master database. Jika metadata tetap berasal dari Shared Statics, seeder tidak diperlukan.

Setelah model selesai, buat migration mengikuti command pada:

```text
Scripts/DotnetEf.txt
```

Periksa hasil migration sebelum diterapkan. Pastikan migration tidak mengubah atau menghapus tabel yang tidak berkaitan.

## 3. Shared Enums and Statics

Tambahkan enum pada:

```text
src/01.Shared/01.Shared.Enums/
```

Enum yang diperlukan:

```text
VendorRegistrationStatus.cs
VendorRegistrationDocumentType.cs
VendorRegistrationStep.cs
```

Status yang disarankan:

```text
Draft
Submitted
UnderVerification
Revision
Verified
Rejected
Cancelled
```

Lengkapi Shared Statics pada:

```text
src/01.Shared/02.Shared.Statics/VendorRegistrations/
```

Statics mencakup:

- Display text
- Maximum length
- Minimum length jika diperlukan
- Questionnaire definitions
- Document evidence definitions
- Status code atau workflow rule bila diperlukan

Enum digunakan untuk data yang terbatas dan konsisten. Jangan menyimpan status sebagai string bebas.

## 4. Shared DTO

Buat DTO pada:

```text
src/01.Shared/03.Shared.Dto/Modules/Main/VendorRegistrations/
```

Struktur yang disarankan:

```text
CheckSapVendor/
StartVendorRegistration/
UpdatePreRegistration/
UpdateQuestionnaire/
UploadVendorRegistrationDocument/
DeleteVendorRegistrationDocument/
GetVendorRegistrationReview/
DownloadVendorRegistrationDocument/
SubmitVendorRegistration/
```

Setiap operasi memiliki DTO sesuai kebutuhan:

- Request
- Response
- Result atau Item
- Route
- Validator

DTO tidak diletakkan pada project FrontEnd WebUi. DTO harus dapat digunakan bersama oleh Backend Logic dan FrontEnd Logic.

Tambahkan validator menggunakan `AbstractValidatorBase<T>`. Validasi mencakup required field, panjang data, email, nomor telepon, enum, jumlah jawaban, dan mandatory document.

## 5. Backend Logic

Buat command dan query pada:

```text
src/02.BackEnd/04.BackEnd.Logics/Modules/Main/VendorRegistrations/
```

Handler yang diperlukan:

```text
CheckSapVendorCommand
StartVendorRegistrationCommand
UpdatePreRegistrationCommand
UpdateQuestionnaireCommand
UploadVendorRegistrationDocumentCommand
DeleteVendorRegistrationDocumentCommand
GetVendorRegistrationReviewQuery
DownloadVendorRegistrationDocumentQuery
SubmitVendorRegistrationCommand
```

Tanggung jawab handler:

- Memvalidasi registration token
- Memvalidasi status registrasi
- Menyimpan draft per tahap
- Menyimpan brand dalam transaction
- Melakukan upsert jawaban questionnaire
- Mengunggah file ke FileStorage
- Menghapus dokumen draft
- Menghasilkan review dari data database
- Menjalankan final validation sebelum submit
- Membuat registration history

Buat service validasi akses registrasi agar logic token tidak diduplikasi pada setiap handler.

## 6. Web API Endpoints

Buat endpoint pada:

```text
src/02.BackEnd/05.BackEnd.WebApi/Modules/Main/VendorRegistrations/
```

Endpoint minimal:

```http
POST   /Main/VendorRegistrations/CheckSapVendor
POST   /Main/VendorRegistrations
PUT    /Main/VendorRegistrations/{id}/PreRegistration
PUT    /Main/VendorRegistrations/{id}/Questionnaire
POST   /Main/VendorRegistrations/{id}/Documents
DELETE /Main/VendorRegistrations/{id}/Documents/{documentId}
GET    /Main/VendorRegistrations/{id}/Review
GET    /Main/VendorRegistrations/{id}/Documents/{documentId}/Download
POST   /Main/VendorRegistrations/{id}/Submit
```

Endpoint registrasi dapat memakai `AllowAnonymous`, tetapi request update wajib membawa opaque registration token, misalnya:

```http
X-Registration-Token: <token>
```

Database hanya menyimpan hash token. Jangan mengizinkan update hanya berdasarkan registration ID karena rentan IDOR.

## 7. FrontEnd Logic

Buat API client command dan query pada:

```text
src/03.FrontEnd/04.FrontEnd.Logics/Modules/Main/VendorRegistrations/
```

Class FrontEnd Logic mengikuti operasi backend:

```text
CheckSapVendorCommand
StartVendorRegistrationCommand
UpdatePreRegistrationCommand
UpdateQuestionnaireCommand
UploadVendorRegistrationDocumentCommand
DeleteVendorRegistrationDocumentCommand
GetVendorRegistrationReviewQuery
DownloadVendorRegistrationDocumentQuery
SubmitVendorRegistrationCommand
```

Gunakan `IBackEndApiService` dan route dari Shared DTO.

Komponen Razor tidak boleh:

- Mengakses database langsung
- Menggunakan DbContext
- Menyimpan binary file sebagai sumber data utama
- Memanggil API menggunakan URL hardcoded

## 8. FrontEnd Integration per Step

Integrasikan halaman yang sudah tersedia pada:

```text
src/03.FrontEnd/05.FrontEnd.WebUi/Modules/Main/Features/VendorRegistrations/
```

Alur integrasi:

### Input SAP

1. Panggil `CheckSapVendor`.
2. Jika valid, panggil `StartVendorRegistration`.
3. Simpan registration ID dan token.
4. Navigasi ke Step 1.

### Step 1

1. Kirim `PreRegistrationRequest`.
2. Backend menyimpan draft dan brand.
3. Navigasi ke Step 2 hanya jika API berhasil.

### Step 2

1. Kirim seluruh questionnaire answers.
2. Backend melakukan upsert per question number.
3. Navigasi ke Step 3 jika API berhasil.

### Step 3

1. Upload setiap file langsung setelah dipilih.
2. Simpan `documentId` dan `fileStorageId` dari response.
3. Hapus dokumen melalui API jika user mengganti atau menghapus file.
4. Jangan mengandalkan `IBrowserFile` setelah upload berhasil.

### Review

1. Ambil data menggunakan `GetVendorRegistrationReviewQuery`.
2. Tampilkan data yang benar-benar tersimpan di backend.
3. Download dokumen melalui endpoint yang tervalidasi.
4. Final submit hanya dijalankan setelah konfirmasi user.

## 9. Draft, Token, File, and Workflow Security

Implementasi wajib memperhatikan:

- Registration token menggunakan random cryptographic token
- Database hanya menyimpan token hash
- Token memiliki expiration
- Token dapat di-revoke
- Setiap endpoint memvalidasi ownership registrasi
- File extension dan MIME type divalidasi
- Ukuran file dibatasi
- File name dinormalisasi
- Malware scanning ditambahkan jika tersedia
- Submitted registration tidak dapat diedit
- Final submit dijalankan dalam transaction
- Setiap perubahan status dicatat dalam history

Status workflow utama:

```text
Draft
→ Submitted
→ UnderVerification
→ Verified / Revision / Rejected
```

Setelah `Verified`, backend dapat membuat atau memperbarui data `Lender` dan user PIC.

## 10. Testing and Verification

Tambahkan test untuk skenario berikut:

- Nomor SAP valid dan tidak valid
- Nomor SAP sudah memiliki registrasi aktif
- Registration token salah atau expired
- Update Step 1 berhasil dan gagal validasi
- Brand utama dan brand tambahan tersimpan
- Questionnaire mandatory belum lengkap
- Sole agent question tidak diwajibkan ketika tidak relevan
- Upload file valid
- Upload extension, MIME type, atau size tidak valid
- Dokumen wajib belum lengkap
- Refresh browser tetap mengambil draft dari backend
- Review menampilkan data database terbaru
- Submit berhasil
- Duplicate submit ditolak
- Submitted registration tidak dapat diedit
- Unauthorized document download ditolak
- History status tercatat
- Transaction rollback ketika submit gagal

Jalankan build dengan mode stabil repository:

```bash
dotnet build EBVL.slnx \
  --no-restore \
  --disable-build-servers \
  -p:UseSharedCompilation=false \
  -nodeReuse:false \
  -m:1
```

Definition of done:

- Migration dapat diterapkan tanpa merusak tabel lain
- Semua endpoint memiliki validator dan access validation
- Draft tersimpan per tahap
- File tersimpan di backend FileStorage
- Review tidak bergantung pada browser state
- Final submit bersifat transaction-safe
- Build dan automated test berhasil
