#!/usr/bin/env bash
# Builds Release and lays out a zip that unpacks straight into the GTA V folder:
#   scripts/AnotherLife.Game.dll (+ dependencies)
#   scripts/AnotherLife/data/*.json
set -euo pipefail
cd "$(dirname "$0")/.."

version="${1:-dev}"
out="dist/AnotherLife-$version"
rm -rf "$out" "$out.zip"
mkdir -p "$out/scripts/AnotherLife/data"

dotnet build src/AnotherLife.Game -c Release --nologo -v quiet
bin="src/AnotherLife.Game/bin/Release/net48"
cp "$bin/AnotherLife.Game.dll" "$bin/AnotherLife.Core.dll" "$bin/Newtonsoft.Json.dll" "$bin/LemonUI.SHVDN3.dll" "$out/scripts/"
cp data/*.json "$out/scripts/AnotherLife/data/"
cp -r licenses "$out/licenses"
cp docs/install.md "$out/INSTALL.md"
cp docs/test-checklist-m0.md "$out/TEST-CHECKLIST.md"
cp docs/controls.md "$out/CONTROLS.md"

python3 -c "import shutil,sys; shutil.make_archive(sys.argv[1], 'zip', sys.argv[1])" "$out"
echo "$out.zip"
