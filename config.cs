using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class appConfig {

	string path = AppDomain.CurrentDomain.BaseDirectory; // Variable para el path de la app ( donde se ejecuta)
	string fileName = "config.json"; // Nombre del archivo
    config appConfigs; // Nueva clase config ( donde se guardan en realidad los ajustes )
    consoleTweaks text; // Chorradas para hacer la consola más bonica

    public appConfig() {
        appConfigs = new config ();
        text = new consoleTweaks();
    }

	// Guardar la configuración de la aplicación en fichero
    private void saveSettings(config data, bool msg = true)
	{
        try
        {
            using (StreamWriter sW = new StreamWriter(path + fileName)) {
                sW.WriteLine(JsonConvert.SerializeObject(data));
            }
            if (msg) {
                text.writeWithColor("Settings have been saved !", ConsoleColor.DarkGreen, true);
            }
        }
        catch (IOException e) {
            text.writeWithColor(e.Message, ConsoleColor.Red, true);
        }
	}

    public void save(bool msg) {
        this.saveSettings (this.appConfigs, msg);
    }

	// Cargar la configuración de la aplicación
	private void loadSettings()
	{
        if (!File.Exists (path + fileName)) {
            File.CreateText (path + fileName).Close ();
            save (false);
        } else {
            using (StreamReader sR = new StreamReader (path + fileName)) {
                appConfigs = JsonConvert.DeserializeObject<config> (sR.ReadLine ());
            }
        }
    }

	public void loadConfig()
	{
        text.writeWithColor("Reading configuration files... ");
        // Asignación de variables
        if (string.IsNullOrEmpty (appConfigs.token) || string.IsNullOrEmpty (appConfigs.passWord)) {
            loadSettings ();
        }
		// Añadidos colorines al OK
        text.writeWithColor("Ok.",ConsoleColor.DarkGreen, true);
	}

    public string getKey() {
        if (!(string.IsNullOrEmpty (appConfigs.token) || string.IsNullOrWhiteSpace (appConfigs.token))) {
            text.writeWithColor ("Using token: ", ConsoleColor.Magenta);
            text.writeWithColor (appConfigs.token, ConsoleColor.DarkYellow, true);
        }
        return appConfigs.token;
    }

    public void setKey(string value) {
        appConfigs.token = value;
        save (false);
    }

    public string getPW() {
        if (!(string.IsNullOrEmpty (appConfigs.passWord) || string.IsNullOrWhiteSpace (appConfigs.passWord))) {
            text.writeWithColor ("Using password: ", ConsoleColor.Magenta);
            text.writeWithColor (appConfigs.passWord, ConsoleColor.DarkYellow, true);
        }
        return appConfigs.passWord;
    }

    public void setPW(string value) {
        appConfigs.passWord = value;
        save (false);
    }

    public List<int> getAuth() {
        if (appConfigs.authID.Count > 0) {
            text.writeWithColor ("AuthID List: ", ConsoleColor.Magenta);
            foreach (int id in appConfigs.authID) {
                text.writeWithColor (id.ToString (), ConsoleColor.DarkYellow);
                text.writeWithColor (",");
            }
            text.writeWithColor ("\n");
        }
        return appConfigs.authID;
    }

    public void setAuth(List<int> value) {
        appConfigs.authID = value;
    }

    private class config {
        public config() {
            token = "";
            passWord = "";
            authID = new List<int>();
        }
        public string token { get; set; }
        public string passWord { get; set; }
        public List<int> authID { get; set; }
	}
}