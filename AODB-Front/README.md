# AODB Frontend - Windows Forms Uygulaması

Bu, Airport Operational Database (AODB) projesinin Windows Forms tabanlı frontend uygulamasıdır.

## Özellikler

- ✅ Kullanıcı girişi (Authentication)
- ✅ Modern ve kullanıcı dostu arayüz
- ✅ Backend API entegrasyonu
- ✅ Keycloak authentication desteği
- 🔄 Uçuş yönetimi (geliştirilme aşamasında)
- 🔄 Havaalanı yönetimi (geliştirilme aşamasında)
- 🔄 Havayolu yönetimi (geliştirilme aşamasında)
- 🔄 Uçak yönetimi (geliştirilme aşamasında)

## Gereksinimler

- .NET 8.0 veya üzeri
- Windows 10/11
- AODB Backend API (çalışır durumda olmalı)

## Kurulum

1. Projeyi klonlayın veya indirin
2. Visual Studio veya JetBrains Rider ile açın
3. NuGet paketlerini geri yükleyin:
   ```
   dotnet restore
   ```

## Yapılandırma

Backend API URL'lerini `Config/AppConfig.cs` dosyasından düzenleyebilirsiniz:

```csharp
public static readonly string BaseApiUrl = "https://localhost:7076/api";
```

## Çalıştırma

1. Backend API'nin çalıştığından emin olun
2. Projeyi derleyin ve çalıştırın:
   ```
   dotnet run
   ```

## Kullanım

1. Uygulama açıldığında login ekranı görünecektir
2. Geçerli kullanıcı adı ve şifre ile giriş yapın
3. Başarılı giriş sonrası ana sayfa açılacaktır
4. İlgili modül butonlarına tıklayarak işlemler yapabilirsiniz

## Proje Yapısı

```
AODB-Front/
├── Models/              # Veri modelleri
├── Services/            # API servis sınıfları
├── Config/              # Uygulama yapılandırma
├── Form1.cs/Designer    # Login formu
├── MainForm.cs/Designer # Ana sayfa formu
└── Program.cs           # Uygulama giriş noktası
```

## Geliştirme Notları

- Form tasarımları Designer ile yapılmıştır
- Async/await pattern kullanılmıştır
- Error handling ve validation mevcuttur
- Modern UI renk paleti kullanılmıştır

## Kullanılan Paketler

- Newtonsoft.Json (JSON serialization)
- .NET 8.0 Windows Forms

## Lisans

Bu proje AODB ekibi tarafından geliştirilmiştir.
