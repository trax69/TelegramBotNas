//
//  botSettings.cs
//
//  Author:
//       Big Cash Monkeys <admin@bcm.ovh>
//
//  Copyright (c) 2017 Big Cash Monkeys
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

using botSettings.Checks;
using botSettings.Enums;

namespace botSettings
{
    /// <summary>
    /// Class used to save Telegram Bot Settings
    /// </summary>
    public class botSetting
    {
        // Private
        private bool isConfigLoaded = false;
        private bool isPathSet = false;
        private string filePathBackup;
        private botConfig conf = new botConfig();

        public botSetting(string filePath)
        {
            configFilePath = filePath;
        }

        /// <summary>
        /// Loads contents of the config file.
        /// Debugs to Console if Error.
        /// </summary>
        /// <returns><c>String</c> to parse as class if config file was loaded, <c>null</c> otherwise.</returns>
        /// <param name="filePath">Path and Name of the file to be loaded.</param>
        private botConfig loadConfFile(string filePath)
        {
            botConfig data = new botConfig();
            Debug.WriteLine("Loading config file.");
            try
            {

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();  // Create the file for the first time
                    saveConfFile(filePath, dataToJson(data));   // Parse the empty class for the first time and Save It.
                }

                using (StreamReader sR = new StreamReader(filePath))
                {
                    filePathBackup = filePath; // crear una copia de la path
                    data = JsonConvert.DeserializeObject<botConfig>(sR.ReadLine());
                    data.configPath = filePathBackup; // volver a escribir la copia tras leer el null
                    isConfigLoaded = true;
                }

            }
            catch (IOException ex)
            {
                Debug.WriteLine("loadConfFile Error: " + ex.Message);
            }
            return data;
        }

        /// <summary>
        /// Saves the config to file path.
        /// Debugs to console if Error.
        /// </summary>
        /// <returns><c>True</c>, if config file was saved, <c>false</c> otherwise.</returns>
        /// <param name="filePath">Path and Name to the file to be saved.</param>
        /// <param name="data">Data to be saved (should be JSON)</param>
        private bool saveConfFile(string filePath, string data)
        {
            bool isSaved = false; // Ponerlo a false
            Debug.WriteLine("Saving to config file.");
            try
            {

                using (StreamWriter sW = new StreamWriter(filePath))
                {
                    sW.WriteLine(data);
                }
                isSaved = true;

            }
            catch (IOException ex)
            {
                Debug.WriteLine("saveConfFile Error: " + ex.Message);
            }
            return isSaved;
        }

        /// <summary>
        /// Class that converts data to <c>String</c>
        /// </summary>
        /// <returns>JSON Parsed string</returns>
        /// <param name="data">Class to be serialized in JSON</param>
        private string dataToJson(botConfig data)
        {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Loads the config from the file path (must include filename in path).
        /// </summary>
        public void loadConfig()
        {
            this.conf = this.loadConfFile(this.conf.configPath);
        }

        /// <summary>
        /// Saves the config from the class to the file (JSON).
        /// </summary>
        public void saveConfig()
        {
            this.saveConfFile(this.conf.configPath, dataToJson(this.conf));
        }

        #region "User Get's and Set's"
        /// <summary>
        /// Adds the an user to the auth list.
        /// </summary>
        /// <returns><c>true</c>, if user was added, <c>false</c> otherwise.</returns>
        /// <param name="ID">UserID (int).</param>
        /// <param name="firstName">User First Name (string).</param>
        /// <param name="lastName">User Last Name (string).</param>
        /// <param name="userName">User's UserName (string).</param>
        /// <param name="isAuth">Is user authenticated (bool) ?</param>
        /// <param name="isAuthProcess">Is user in authentication process (bool) ?</param>
        /// <param name="nrTry">User Number of trys (int).</param>
        /// <param name="rol">User Role (usrRole).</param>
        public void addUser(int ID, string firstName = "", string lastName = "", string userName = "",
        bool isAuth = false, bool isAuthProcess = false, int lastMsgID = -1,
        int nrTry = 3, usrRole rol = usrRole.Normal)
        {
            var data = new botUser();
            data.iD = ID;
            data.firstName = firstName;
            data.lastName = lastName;
            data.userName = userName;
            data.isAuth = isAuth;
            data.inputText = isAuthProcess;
            data.banUntil = DateTime.Now;
            data.nrTry = nrTry;
            data.role = rol;

            try
            {
                conf.authList.Add(data);
                Debug.WriteLine("New user added to the authList");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("addUser Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Checks if the user is already in the auth list
        /// </summary>
        /// <returns>The userID in auth list, -1 otherwise</returns>
        /// <param name="ID">UserID (int).</param>
        public int getIndexAuthList(int ID)
        {
            int data;
            data = conf.authList.FindIndex(botUser => botUser.iD == ID);
            Debug.WriteLine("Is user auth in list: (int) " + data);
            return data;
        }

        /// <summary>
        /// Get's the user from the list so you change it's propertys
        /// </summary>
        /// <param name="listIndex">Index of the user in the list element</param>
        /// <returns>Returns an <see cref="botUser"/> object</returns>
        public botUser userPropertys(int listIndex)
        {
            return conf.authList[listIndex];
        }
        #endregion

        #region "Config Get's and Set's"
        /// <summary>
        /// Gets or sets the bot token.
        /// </summary>
        /// <value>The bot token.</value>
        public string botToken
		{
			get {
				if (!isConfigLoaded) {
					if (!isPathSet) {
						throw new IOException ("Please set a config file path");
					}
					loadConfig ();
				}
				return conf.token;
			}
			set {
				conf.token = value;
			}
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string passWord
		{
			get {
				if (!isConfigLoaded) {
					if (!isPathSet) {
						throw new IOException ("Please set a config file path");
					}
					loadConfig ();
				}
				return conf.authPW;
			}
			set {
				conf.authPW = value;
			}
		}

		/// <summary>
		/// Gets or sets the config filepath.
		/// </summary>
		/// <value>The config filepath including filename.</value>
		public string configFilePath
		{
			get {
				if (!isConfigLoaded) {
					if (!isPathSet) {
						throw new IOException ("Please set a config file path");
					}
					loadConfig ();
				}
				return conf.configPath;
			}
			set {
				conf.configPath = value;
				isPathSet = true;
			}
		}

		/// <summary>
		/// Gets or sets the torrent files path.
		/// </summary>
		/// <value>The torrent files path.</value>
		public string torrentFilesPath
		{
			get {
				if (!isConfigLoaded) {
					if (!isPathSet) {
						throw new IOException ("Please set the torrent directory path");
					}
					loadConfig ();
				}
				return conf.torrentPath;
			}
			set {
				conf.torrentPath = value;
			}
		}

        /// <summary>
        /// Gets or sets the torrent files path.
        /// </summary>
        /// <value>The torrent files path.</value>
        public string incompleteFilesPath
        {
            get
            {
                if (!isConfigLoaded)
                {
                    if (!isPathSet)
                    {
                        throw new IOException("Please set the incomplete directory path");
                    }
                    loadConfig();
                }
                return conf.incompletePath;
            }
            set
            {
                conf.incompletePath = value;
            }
        }


        /// <summary>
        /// Gets or sets the torrent files path.
        /// </summary>
        /// <value>The torrent files path.</value>
        public string completedFilesPath
        {
            get
            {
                if (!isConfigLoaded)
                {
                    if (!isPathSet)
                    {
                        throw new IOException("Please set the completed directory path");
                    }
                    loadConfig();
                }
                return conf.completedPath;
            }
            set
            {
                conf.completedPath = value;
            }
        }
#endregion

        /// <summary>
        /// Class where all the app settings are going to be saved to we can serialize to JSON
        /// </summary>
        private class botConfig
		{
			public string token { get; set; }
			public string authPW { get; set; }
			public string configPath { get; set; }
            public string torrentPath { get; set; }
            public string incompletePath { get; set; }
            public string completedPath { get; set; }



            public List<botUser> authList { get; set; }

			public botConfig()
			{
				authList = new List<botUser>();
			}
		}
	}

	/// <summary>
	/// Class that can manage Telegram Bot Users to make Permission System
	/// </summary>
	public class botUser
	{
		public int iD { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string userName { get; set; }
		/// <summary>
		/// Gets or sets the number of trys that the user has before he get's banned from the auth system
		/// </summary>
		/// <value>The number of trys</value>
		public int nrTry { get; set; }
		public bool inputText { get; set; }
		public bool isAuth { get; set; }
		public DateTime banUntil { get; set; }
		/// <summary>
		/// Gets or sets the user role in the bot.
		/// </summary>
		/// <value>The user role from enum <c>usrRole</c>.</value>
		public usrRole role { get; set; }
	}
}