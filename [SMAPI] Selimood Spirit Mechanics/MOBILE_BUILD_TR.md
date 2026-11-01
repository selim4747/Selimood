# Mobilde kullanım için derleme rehberi

## Hangi dosyayı alacağım?

Derleme bittikten sonra alınacak dosya şudur:

```text
Selimood.SpiritMechanics.dll
```

Genellikle şu klasörde oluşur:

```text
bin/Release/net6.0/Selimood.SpiritMechanics.dll
```

Android’e `.cs` veya `.csproj` dosyasını koymak modun çalışması için yeterli değildir. Android SMAPI’nin yüklediği dosya derlenmiş `.dll` dosyasıdır.

## Önerilen yöntem: bilgisayarda derle, telefona kopyala

1. `Selimood.SpiritMechanics.csproj`, `ModEntry.cs` ve helper mod klasörünü bilgisayara çıkar.
2. Bilgisayarda .NET SDK kurulu olsun.
3. `StardewPath` değişkenini `StardewModdingAPI.dll` ve `Stardew Valley.dll` dosyalarının bulunduğu klasöre yönlendir.
4. Kaynak klasöründe terminal açıp şu komutu çalıştır:

```bash
 dotnet build Selimood.SpiritMechanics.csproj -c Release -p:StardewPath="/Stardew Valley"
```

Windows örneği:

```powershell
 dotnet build .\Selimood.SpiritMechanics.csproj -c Release -p:StardewPath="C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley"
```

5. Şu DLL’yi al:

```text
bin/Release/net6.0/Selimood.SpiritMechanics.dll
```

6. Android telefondaki eski DLL’nin yerine bu yeni DLL’yi kopyala. `manifest.json` ve diğer mod dosyaları aynı klasörde kalmalı.
7. Oyunu açıp SMAPI logunda şu mesajı ara:

```text
Ruh Çiçeği seyahat helper kaynak projesi yüklendi.
```

## Telefonda doğrudan derleme

Android’de Termux ile .NET SDK kurmak teorik olarak mümkün olsa da Stardew Valley ve SMAPI referans DLL’lerine erişim çoğu kurulumda sorun çıkarır. Bu pakete `build_android_termux.sh` betiği eklendi; betik referans DLL’leri bulursa derlemeyi telefonda başlatır. Betik bunları bulamazsa hata verip hangi dosyaların eksik olduğunu gösterir.

Telefonunda derleme yapmayı denersen gerekenler:

- Termux
- Uyumlu .NET SDK
- `StardewModdingAPI.dll`
- `Stardew Valley.dll`
- Kaynak proje dosyaları

Referans DLL’leri olmadan `dotnet build` şu tür bir hata verir: `The type or namespace name StardewValley could not be found`.

### Termux betiği

Kaynak klasöründe Termux açıp betiği çalıştır:

```bash
chmod +x build_android_termux.sh
mkdir -p "$HOME/stardew-refs"
./build_android_termux.sh
```

Betik iki referans dosyasını burada arar:

```text
$HOME/stardew-refs/StardewModdingAPI.dll
$HOME/stardew-refs/Stardew Valley.dll
```

Başka bir klasör kullanacaksan:

```bash
STARDew_PATH="/dosya/yolu" ./build_android_termux.sh
```

> **Not:** SMAPI APK’sının kendisi kaynak DLL referansları değildir. APK’yı bu klasöre koymak işe yaramaz; derleme için yukarıdaki iki `.dll` dosyası gerekir. Android’in uygulama özel klasörleri erişime kapalıysa Termux bunları okuyamaz. Bu durumda telefon üzerinde derleme yapılamaz; yalnızca derlenmiş DLL telefona kopyalanabilir.

Betiğin başarılı çıktısı `Selimood.SpiritMechanics.new.dll` olur. Bu dosyayı `Selimood.SpiritMechanics.dll` adıyla mod klasöründeki eski DLL’nin yerine koy.

## Android’e hangi klasöre koyacağım?

Android SMAPI kurulumuna göre mod klasörü değişebilir. SMAPI’nin kullandığı Mods klasöründe şu klasörü bul:

```text
[SMAPI] Selimood Spirit Mechanics
```

İçindeki eski:

```text
Selimood.SpiritMechanics.dll
```

dosyasını yeni derlenen DLL ile değiştir. `[CP] Selimood` klasörüne dokunma.

## Android test sırası

1. Oyunu aç ve SMAPI logunda helper yüklenme mesajını kontrol et.
2. Çiftlikteki çiçek standından Tuhaf Ruh Çiçeği al.
3. Çiçeği doğrudan tüketmeyi dene.
4. E25 Ruh Bahçesi Hub’ına `(46,14)` civarında gelmelisin.
5. E25 içinde normal Ruh Çiçeği tüket.
6. Selim çiftliğine `(25,16)` civarında dönmelisin.
7. Çalışmazsa SMAPI logunu kontrol et.

İlk denemede `RequireWhiteStoneUnlock=false` bırakıldığı için 240 Beyaz Taş beklenmez. 240 taş kilidini test etmek için helper modunun oluşturduğu `config.json` dosyasında bu değeri `true` yapabilirsin.
