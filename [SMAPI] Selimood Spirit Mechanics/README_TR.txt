Selimood Ruh Çiçeği Mekanikleri (Android SMAPI helper)

Bu klasör artık hem mevcut derlenmiş DLL’yi hem de kaynak SMAPI projesini içerir.

Kaynak dosyalar:
- Selimood.SpiritMechanics.csproj
- ModEntry.cs

Proje Stardew/SMAPI 4.3.2.x hedefiyle net6.0 için hazırlanmıştır. Derlemek için StardewPath değişkenini StardewModdingAPI.dll ve Stardew Valley.dll dosyalarının bulunduğu klasöre yönlendir:

 dotnet build Selimood.SpiritMechanics.csproj -p:StardewPath="/Stardew Valley"

Derleme çıktısındaki Selimood.SpiritMechanics.dll dosyasını bu klasördeki eski DLL’nin yerine koy. Android’de DLL’yi doğrudan telefonda derlemek yerine bilgisayarda derleyip Mods klasörüne kopyalamak gerekir.

Mekanikler:
- Tuhaf Ruh Çiçeği tüketilince E25 Ruh Bahçesi’ne geçiş.
- E25 bölgesinde normal Ruh Çiçeği tüketilince Selim çiftliğine dönüş.
- En az 240 Beyaz Taş Parçası bulunduğunda kayıt bazlı sınırsız E25 seyahati.
- /e25_status: mevcut seyahat durumunu log’a yazar.
- /e25_unlock: test için sınırsız E25 seyahatini açar.

Deneme ayarı varsayılan olarak RequireWhiteStoneUnlock=false’dur; böylece Tuhaf Ruh Çiçeği ile ilk test doğrudan yapılabilir. Gerçek 240 taş kilidini test etmek için config.json içinde true yapabilirsin.

Mevcut derlenmiş DLL eski sürüm olabilir. Kaynak projeyi derlemeden önce eski DLL’nin Tuhaf Ruh Çiçeği seyahatini desteklediği varsayılmamalıdır.

Kurulum: Bu klasörü ve [CP] Selimood klasörünü StardewValley/Mods içine çıkarın. Content Patcher ve Android SMAPI 4.3.2.5 veya üzeri gerekir.

Mevcut ekin davranışı:
- 24. gün: Ruh Çiçeği + 1 Ruh Çiçeği Tohumu
- 25. gün: Tuhaf Ruh Çiçeği + 2 Ruh Çiçeği Tohumu
- 26. gün ve sonrası: Ölü Çiçek + 1 Çürük Tohum
- Çürük Tohum her mevsimde ekilebilir ve helper tarafından büyümesi durdurulur.
