using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class appConfig {

	string path = AppDomain.CurrentDomain.BaseDirectory; // Variable para el path de la app ( donde se ejecuta)
	string fileName = "config.json";

	// Guardar la configuración de la aplicación
	public void saveSettings(string index, string value)
	{
		IDictionary<string,string> data = new Dictionary<string, string>();
		data [index] = value;
		var json = JsonConvert.SerializeObject (data);
		using (StreamWriter file = new StreamWriter (path + fileName,true)) {
			file.WriteLine (json);
		}
		/*
		Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		config.AppSettings.Settings.Remove(index);
		config.AppSettings.Settings.Add(index, value);
		config.Save(ConfigurationSaveMode.Modified);
		*/
	}

	// Cargar la configuración de la aplicación
	private string loadSetting(string index)
	{
		return "";
	}

	public string loadConfig(string index)
	{
		Console.Write("Reading configuration files... ");
		// Asignación de variables
		//botKey = loadSetting("key");
		//pW = loadSetting("pW");
		// Añadidos colorines al OK
		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("OK.");
		Console.ResetColor();
		return loadSetting(index);
	}
}