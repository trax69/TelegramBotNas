#!/bin/bash

paquete="$(dpkg -l | grep mono-complete)"
echo "----------------------------"
echo "Comprobando las dependencias"
echo "----------------------------"
sleep 1
if [ -z "$paquete" ]
then
echo "Mono NO está instalado vamos a proceder a instalarlo"
sleep 3
apt-get update && apt-get install mono-complete
else
echo "Mono está instalado continuamos con la compilación"
fi

sleep 1
echo "----------------------------"
echo "Procediendo a parchear los archivos y posterior compilado"
### POR AHORA NO ES NECESARIO Parchear el  archivo nasBot.csproj modificando ToolsVersion=12
### sed -i 's/ToolsVersion="4.0"/ToolsVersion="12"/g' nasBot.csproj
echo "----------------------------"
sleep 1
echo "Compilando..."
echo "----------------------------"
# Compilar 
xbuild nasBot.sln

echo "----------------------------"
echo "Importando los certificados ssl de telegram para poder funcioanr"
echo "----------------------------"
#Una  vez compilado tenemos que importar los certificados ssl
#Comprobar si esta instalado
certmgr -ssl https://telegram.org/
#Cambiar el nombre de la carpeta 
mv Debug Compiled

echo "----------------------------" 
echo "Compilacion finalizada"
echo "----------------------------"
#Finalizamos script
exit
