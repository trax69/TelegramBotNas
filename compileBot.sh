#!/bin/bash

# Copy System.dll
cp packages/Microsoft.AspNet.WebApi.Client.5.2.3/lib/net45/System.Net.Http.Formatting.dll .

# Copy Json.dll
cp packages/Newtonsoft.Json.9.0.1/lib/net45/Newtonsoft.Json.dll .

# Copy Telegram.dll
cp packages/Telegram.Bot.10.4.0/lib/net45/Telegram.Bot.dll .

# Create botSettings.dll
cd botSettings
dmcs -o "botSettings.dll" -t:library -r:../Newtonsoft.Json.dll botSettings.cs botSettingsChecks.cs botSettingsEnums.cs
cd ..

# Copy botSettings.dll
cp botSettings/botSettings.dll .

# Compile nasBot
dmcs -o "nasBot" -r:System.Net.Http.Formatting.dll -r:botSettings.dll -r:Newtonsoft.Json.dll -r:Telegram.Bot.dll Program.cs consoleTweaks.cs
