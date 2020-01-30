#!/bin/bash

while read -r line; do declare "$line"; done < version.txt

sed -i 's/<version>/$Build/g' manifest.json
sed -i 's/<version>/$Build/g' "$2.cs"