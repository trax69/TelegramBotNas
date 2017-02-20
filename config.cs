using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class appConfig {

	string path = AppDomain.CurrentDomain.BaseDirectory; // Variable para el path de la app ( donde se ejecuta)
	string fileName = "config.json"; // Nombre del archivo
    config appConfigs = new config(); // Nueva clase config ( donde se guardan en realidad los ajustes )
    consoleTweaks text = new consoleTweaks(); // Chorradas para hacer la consola más bonica

    /* Para guardar los ajustes */
	public void setSetting(string index, string value) {
        if (index == "key")
        {
            this.appConfigs.token = value;
            text.writeWithColor("Token set !", ConsoleColor.DarkGreen, true);
        }
        else if (index == "pW") {
            this.appConfigs.passWord = value;
            text.writeWithColor("Password set !", ConsoleColor.DarkGreen, true);
        }
        this.saveSettings();
	}

    /* Function overload -> para guardar los ajustes si detecta que es una lista */
    public void setSetting(string index, List<int> value)
    {
        if (index == "authID")
        {
            this.appConfigs.authID = value;
            text.writeWithColor("AuthID set !", ConsoleColor.DarkGreen, true);
        }
        this.saveSettings();
    }

	// Guardar la configuración de la aplicación en fichero
	private void saveSettings()
	{
        try
        {
            using (StreamWriter sW = new StreamWriter(path + fileName))
            {
                string json = JsonConvert.SerializeObject(this.appConfigs);
                sW.WriteLine(json);
                sW.Close();
            }
            text.writeWithColor("Settings have been saved !", ConsoleColor.DarkGreen, true);
        }
        catch (IOException e) {
            text.writeWithColor(e.Message, ConsoleColor.Red, true);
        }
	}

	// Cargar la configuración de la aplicación
	private void loadSetting()
	{
        try
        {
            if (!File.Exists(path + fileName))
            {
                File.CreateText(path + fileName).Close();
            }
            using (StreamReader sR = new StreamReader(path + fileName))
            {
                this.appConfigs = JsonConvert.DeserializeObject<config>(sR.ReadLine());
                sR.Close();
            }
        } catch (ArgumentNullException) {
            this.appConfigs.token = "";
            this.appConfigs.passWord = "";
            this.appConfigs.authID = new List<int>();
        }
	}

	public void loadConfig()
	{
		Console.Write("Reading configuration files... ");
        // Asignación de variables
        loadSetting();
		// Añadidos colorines al OK
        text.writeWithColor("Ok.",ConsoleColor.DarkGreen, true);
	}

    public string getKey() {
        if (this.appConfigs.token == null)
        {
            loadSetting();
        }
        return this.appConfigs.token;
    }

    public string getPW() {
        if (this.appConfigs.passWord == null)
        {
            loadSetting();
        }
        return this.appConfigs.passWord;
    }

    public List<int> getAuth() {
        if (this.appConfigs.authID == null) {
            loadSetting();
        }
        return this.appConfigs.authID;
    }

	private class config {
		public string token;
		public string passWord;
		public List<int> authID = new List<int>();
	}
}