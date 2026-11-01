# Selimood v0.4.0 — oda mobilya ölçeği düzeltmesi

Stardew Valley 1.6 için Content Patcher NPC paketi. Bu sürümde oda içindeki mobilya ve dekorlar Selim’in mevcut sprite ölçeğine göre küçültüldü; oda boyutu, tile ölçüsü, kapı yeri, çiftlik haritası ve karakter sprite’ı değiştirilmedi. Ruh Çiçeği hasat kuralları için paketteki ayrı C# yardımcı mod da gerekir.

## Kurulum

1. ZIP içindeki `[CP] Selimood` ve `[SMAPI] Selimood Spirit Mechanics` klasörlerini `StardewValley/Mods` içine kopyala.
2. Content Patcher’ın ve Android kullanıyorsan kullandığın oyun/SMAPI sürümüyle uyumlu Android SMAPI’nin kurulu olduğundan emin ol.
3. Mevcut kayıt dosyanı yedekle; özellikle yeni çiftlik/ev konumuna ilk kez girerken.

## Yenilenen haritalar

- Selim’in çiftliği 50×32 tile; orman çevresi, ev, patika, küçük ekim alanı ve gölet/şelale düzeni içerir.
- Ev içi 34×29 tile olarak kaldı. Mobilyalar küçültüldü ve aralarındaki açık yürüme alanı artırıldı; sıcak ahşap zemin, yatak, kitaplık, çalışma masası/bilgisayar ve bitkiler korundu.
- Haritalarda 16×16 tile düzeni, görünmez çarpışma hücreleri ve nesnelerin oyuncunun önünde çizilmesini sağlayan Front katmanı kullanılır. Gölet hücreleri su olarak işaretlidir; bahçe alanındaki 64 tile `Diggable` özelliği taşır.
- Standart Çiftlik’ten Selim’in çiftliğine giriş ve çiftlik-ev-çiftlik geçişleri yeni kapı koordinatlarına bağlandı.

## Ruh Çiçeği

- Ruh Çiçeği Tohumu ilkbahar, sonbahar ve kışın ekilebilir; yazın ekilemez. Normal büyüme süresi toplam 24 gündür.
- Ekimden 24 gün sonra hasat: 1 Ruh Çiçeği + 1 Ruh Çiçeği Tohumu.
- Ekimden 25 gün sonra hasat: 1 Tuhaf Ruh Çiçeği + 2 Ruh Çiçeği Tohumu.
- Ekimden 26 gün sonra veya daha geç hasat: 1 Ölü Çiçek + 1 Çürük Tohum.
- Çürük Tohum dört mevsimde de ekilebilir ve helper tarafından büyümesi sürekli durdurulur.

## Teknik not

Haritalar TMX/XML yapısı, tile atlas boyutları, Content Patcher JSON’u ve warp koordinatları paketlenmeden önce doğrulandı. Oyun/Android içinde canlı SMAPI testi bu ortamda yapılamadı; Android SMAPI deneysel olabilir. Bir hata çıkarsa SMAPI günlük dosyası gerekir.
## Geçici sprite durumu
Bu sürümde Selim’in özel karakter spritesheet yüklemesi geçici olarak kaldırılmıştır. Portre ve NPC verileri korunur; oyun içi hareketli karakter görünümü için ileride geçerli bir Stardew Valley spritesheet’i eklenmelidir.
## Yeni çiftlik haritası
Gönderilen 800×512 görsel 50×32 tile işlevsel haritaya dönüştürüldü. Çiftlik girişi (3,14), ev kapısı (42,10), dönüş noktası ve ekilebilir alan yeni düzene göre ayarlandı.
## Karakter görünümü
Selim’in gönderilen 16×32 karesi, animasyon olmaması için dört yön ve dört kare boyunca aynı görüntünün tekrarlandığı statik spritesheet olarak kullanılır.
## Hediye tercihleri
Tuhaf Ruh Çiçeği favori; normal Ruh Çiçeği sevilen hediye olarak tanımlıdır.
## E25 prototipi
E25 için düz 16×16 karelerden oluşan geçici Ruh Bahçesi, Tenti Madeni, Tömye Köyü, Kütüphane, Heyer Çiftliği ve Evren Girişi haritaları eklendi. Dört ayrı Selim kimliği ve Mutlu Selim’in eşi şimdilik placeholder olarak tanımlıdır; diyaloglar daha sonra doldurulacaktır.
## Tömye güncellemesi
Tömye Köyü 7 ev, Egir Marketi ve L25 laboratuvarı olarak düzenlendi. Egir ve Kyldo placeholder NPC olarak eklendi. Tüm geçici karakter sprite’ları mevcut Selim sprite’ının ayrı kopyalarıdır.
## Oynanabilir prototip güncellemesi
E25 haritalarına desenli 16×16 prototip tileset, statik sprite yürüyüş programları ve Ruh Bahçesi deneme dükkânı eklendi. Ruh Çiçeği Tohumu ve Tuhaf Ruh Çiçeği Tohumu deneme stoğunda 1 altındır.
## Çiftlik giriş düzeltmesi
Selim çiftliği girişi sağ taraftaki yol koridoruna taşındı. Standart çiftlikten giriş `(79,14)`, Selim çiftliğinde güvenli varış `(46,14)` ve geri dönüş `(49,14)` olarak ayarlandı.
## v0.5.0 işlevsel düzeltmeleri
Ev kapısı çarpışması, E25 çift yönlü bağlantıları, NPC schedule yapısı, E25 test erişimi, E25 placeholder portreleri ve Selim çiftliğinde 1 altınlık Ruh Çiçeği/Tuhaf Ruh Çiçeği standı eklendi.
## SMAPI kaynak projesi
`[SMAPI] Selimood Spirit Mechanics` klasörüne `Selimood.SpiritMechanics.csproj` ve `ModEntry.cs` eklendi. Tuhaf Ruh Çiçeği ile E25 seyahati, normal çiçekle dönüş ve 240 Beyaz Taş açılımı bu kaynak projededir. Derleme için README_TR.txt içindeki StardewPath komutunu kullanın.
