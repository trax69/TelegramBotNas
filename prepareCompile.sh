#!/bin/bash

# Copy System Dll
cp packages/Microsoft.AspNet.WebApi.Client.5.2.3/lib/net45/System.Net.Http.Formatting.dll .

# Copy Json Dll
cp packages/Newtonsoft.Json.9.0.1/lib/net45/Newtonsoft.Json.dll .

# Copy Telegram Dll
cp packages/Telegram.Bot.10.4.0/lib/net45/Telegram.Bot.dll .

# Compile the program
dmcs -out "nasBot" -r:System.Net.Http.Formatting.dll -r:Newtonsoft.Json.dll -r:Telegram.Bot.dll Program.cs checks.cs config.cs consoleTweaks.cs
