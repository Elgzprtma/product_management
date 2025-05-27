# NawatechTest

[![.NET](https://github.com/username/repo/actions/workflows/dotnet.yml/badge.svg)](https://github.com/username/repo/actions/workflows/dotnet.yml)

## Deskripsi

Ini aplikasi web berbasis ASP.NET Core MVC untuk manajemen produk dan kategori, lengkap dengan fitur upload gambar produk, validasi input, dan soft delete.

## Fitur

- CRUD Produk  
  - Tambah, edit, hapus produk  
  - Upload gambar produk  
- CRUD Kategori  
  - Tambah, edit, hapus kategori (soft delete)  
- Validasi input pada form  
- Soft Delete agar data tidak langsung hilang  
- Tampilan daftar produk lengkap dengan kategori

## Prasyarat

- .NET SDK versi terbaru  
- SQL Server / Database yang sudah dikonfigurasi : Postgres
- Visual Studio / VS Code

## Instalasi dan Setup

1. Clone repository:  
   ```bash
   [git clone https://github.com/username/repo.git](https://github.com/Elgzprtma/product_management
   cd repo

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NawatechDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Update database dengan EF Core migrations:


dotnet ef database update
Jalankan aplikasi:


dotnet run



Struktur Folder
/Controllers  - Controller MVC  
/Models       - Model entity  
/Views        - Razor views  
/wwwroot/uploads/products - Folder penyimpanan gambar produk  
