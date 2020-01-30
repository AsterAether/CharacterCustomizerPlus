#!/bin/bash

if [[ ! -d "$1$2/build/" ]]
then 
    mkdir "$1$2/build/"
fi

cp -f "$1$2/bin/netstandard2.0/$3.dll" "$1$2/build/$3.dll";

cp -f "$1$2/manifest.json" "$1$2/build/manifest.json";

cp -f "$1$2/icon.png" "$1$2/build/icon.png";

cp -f "$1README.md" "$1$2/build/README.md";

cp -f "$1LICENSE.txt" "$1$2/build/LICENSE.txt";

zip -r -j "$1$2/build/build.zip" "$1$2/build/" -x "$1$2/build/build.zip"

while read -r line; do declare "$line"; done < version.txt

sed -i 's/$Build/<version>/g' manifest.json
sed -i 's/$Build/<version>/g' "$2.cs"