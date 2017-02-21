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

	// Guardar la configuración de la aplicación en fichero
	private void saveSettings()
	{
        try
        {
            using (StreamWriter sW = new StreamWriter(path + fileName)) {
                sW.WriteLine(JsonConvert.SerializeObject(appConfigs));
            }
            text.writeWithColor("Settings have been saved !", ConsoleColor.DarkGreen, true);
        }
        catch (IOException e) {
            text.writeWithColor(e.Message, ConsoleColor.Red, true);
        }
	}

	// Cargar la configuración de la aplicación
	private void loadSettings()
	{
        if (!File.Exists (path + fileName)) {
            File.CreateText (path + fileName).Close ();
            using (StreamWriter sW = new StreamWriter (path + fileName)) {
                sW.WriteLine (JsonConvert.SerializeObject (appConfigs));
            }
        } else {
            using (StreamReader sR = new StreamReader (path + fileName)) {
                appConfigs = JsonConvert.DeserializeObject<config> (sR.ReadLine ());
            }
        }
    }

	public void loadConfig()
	{
		Console.Write("Reading configuration files... ");
        // Asignación de variables
        if (string.IsNullOrEmpty(appConfigs.token) || string.IsNullOrEmpty(appConfigs.passWord)) {
            loadSettings ();
        }
		// Añadidos colorines al OK
        text.writeWithColor("Ok.",ConsoleColor.DarkGreen, true);
	}

    public string getKey() {
        return appConfigs.token;
    }

    public void saveKey(string value) {
        appConfigs.token = value;
        saveSettings ();
    }

    public string getPW() {
        return appConfigs.passWord;
    }

    public void savePW(string value) {
        appConfigs.passWord = value;
        saveSettings ();
    }

    public List<int> getAuth() {
        return appConfigs.authID;
    }

    public void saveAuth(List<int> value) {
        appConfigs.authID = value;
        saveSettings ();
    }

	private class config {
        private string _token;
        private string _passWord;
        private List<int> _auth;

        public string token {
            get { 
                return this._token;
            } set { 
                this._token = value;
            }
        }

        public string passWord { 
            get { 
                return this._passWord;
            } set { 
                this._passWord = value;
            }
        }

        public List<int> authID { 
            get { 
                return this._auth;
            } set { 
                this._auth = value;
            }
        }
	}
}