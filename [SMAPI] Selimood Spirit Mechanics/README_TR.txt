Selimood Ruh Çiçeği Mekanikleri (Android SMAPI helper)

Bu klasör artık hem mevcut derlenmiş DLL’yi hem de kaynak SMAPI projesini içerir.

Kaynak dosyalar:
- Selimood.SpiritMechanics.csproj
- ModEntry.cs

Proje Stardew/SMAPI 4.3.2.x hedefiyle net9.0 için hazırlanmıştır. Derlemek için StardewPath değişkenini StardewModdingAPI.dll ve Stardew Valley.dll dosyalarının bulunduğu klasöre yönlendir:

 dotnet build Selimood.SpiritMechanics.csproj -p:StardewPath="/Stardew Valley"

Derleme çıktısındaki Selimood.SpiritMechanics.dll dosyasını bu klasördeki eski DLL’nin yerine koy. Android’de DLL’yi doğrudan telefonda derlemek yerine bilgisayarda derleyip Mods klasörüne kopyalamak gerekir.

Mekanikler:
- Tuhaf Ruh Çiçeği tüketilince E25 Ruh Bahçesi’ne geçiş.
- E25 bölgesinde normal Ruh Çiçeği tüketilince Selim çiftliğine dönüş.
- Toplam 240 Beyaz Taş Parçası biriktirildiğinde kayıt bazlı sınırsız E25 seyahati.
- EgirMarket dükkânından alınan tarifle 240 Beyaz Taş Parçası kullanarak Beyaz Taş üretimi.
- /e25_status: mevcut seyahat durumunu log’a yazar.
- /e25_unlock: test için sınırsız E25 seyahatini açar.

RequireWhiteStoneUnlock varsayılan olarak true’dur. İlk Tuhaf Ruh Çiçeği kullanımı ücretsizdir; sonraki kullanımlar toplam 240 parça eşiğini ister.

Mevcut derlenmiş DLL eski sürüm olabilir. Kaynak projeyi derlemeden önce eski DLL’nin Tuhaf Ruh Çiçeği seyahatini desteklediği varsayılmamalıdır.

Kurulum: Bu klasörü ve [CP] Selimood klasörünü StardewValley/Mods içine çıkarın. Content Patcher ve Android SMAPI 4.3.2.5 veya üzeri gerekir.

Mevcut ekin davranışı:
- 24. gün: Ruh Çiçeği + 1 Ruh Çiçeği Tohumu
- 25. gün: Tuhaf Ruh Çiçeği + 2 Ruh Çiçeği Tohumu
- 26. gün ve sonrası: Ölü Çiçek + 1 Çürük Tohum
- Çürük Tohum oyunda Turfanda Tohumu adıyla görünür; Turfanda, E25’in beşinci mevsimidir. E25 tasarımında her zaman Turfanda mevsimi kabul edilir.
- Turfanda Tohumu her normal mevsimde teknik olarak ekilebilir ve helper tarafından büyümesi durdurulur; özel beşinci mevsim görünümü için daha sonra ayrı zaman/crop katmanı eklenebilir.
