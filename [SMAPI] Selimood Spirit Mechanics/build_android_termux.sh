#!/data/data/com.termux/files/usr/bin/bash
set -eu

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
REFS_DIR="${STARDew_PATH:-${STARDewPath:-$HOME/stardew-refs}}"
PROJECT="$SCRIPT_DIR/Selimood.SpiritMechanics.csproj"
OUT="$SCRIPT_DIR/bin/Release/net9.0/Selimood.SpiritMechanics.dll"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet bulunamadı. Termux'ta uygun .NET SDK'yı kur veya mevcut dotnet yolunu PATH'e ekle."
  echo "Örnek: pkg update && pkg install dotnet-sdk-8.0"
  exit 1
fi

if [ ! -f "$REFS_DIR/StardewModdingAPI.dll" ] || [ ! -f "$REFS_DIR/Stardew Valley.dll" ]; then
  echo "Eksik referans DLL'leri:"
  echo "  $REFS_DIR/StardewModdingAPI.dll"
  echo "  $REFS_DIR/Stardew Valley.dll"
  echo
  echo "Bu iki dosyayı telefon erişebiliyorsa $REFS_DIR klasörüne koy."
  echo "Farklı klasör kullanıyorsan şu şekilde çalıştır:"
  echo "  STARDew_PATH=/dosya/yolu ./build_android_termux.sh"
  exit 2
fi

echo "Kaynak proje derleniyor..."
dotnet build "$PROJECT" -c Release -p:StardewPath="$REFS_DIR"

if [ ! -f "$OUT" ]; then
  echo "Derleme tamamlandı görünüyor fakat DLL bulunamadı: $OUT"
  exit 3
fi

cp "$OUT" "$SCRIPT_DIR/Selimood.SpiritMechanics.new.dll"
echo
echo "Hazır DLL:"
echo "  $SCRIPT_DIR/Selimood.SpiritMechanics.new.dll"
echo
echo "Bu dosyayı mevcut Selimood.SpiritMechanics.dll dosyasının yerine koy."
