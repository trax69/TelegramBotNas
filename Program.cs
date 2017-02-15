using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Configuration;
using Telegram.Bot;

namespace nasBot {
	
	class nasBot {

		private string botKey = "";
		private bool progLoop = true;
		public static void Main(string[] args) {

			/* Crear una instancia del objeto para poder acceder a los metodos privados */
			nasBot obj = new nasBot ();

			/* Recorrer los argumentos pasados a la app */
			foreach (string arg in args) {
				// Si los argumentos contienen un igual EJ: Key=Value
				if (arg.Contains ("=")) {
					// Separa el string en un array diviendo por el '='
					string[] data = arg.Split ((char)'=');
					// Si la primera parte del array contiene 'key', asignamos en los ajustes
					if (data [0].Contains ("key")) {
						obj.saveSettings ("key", data[1]);
						// Salir del programa, no necesitamos que siga, solo es para escribir la clave
						Console.WriteLine("The key has been successfully saved !");
						Environment.Exit (0);
					}
				}
			}

			/* Dar la bienvenida al usuario :-) */
			Console.WriteLine("Welcome to Telegram Bot App !");

			/* Cargar configuración del bot */
			obj.loadConfig (obj);

			Telegram.Bot.TelegramBotClient bot = new TelegramBotClient (obj.botKey);
			bot.StartReceiving ();
			bot.OnMessage += Bot_OnMessage;

			while (obj.progLoop) {
				System.Threading.Thread.Sleep (10000);
			}

			bot.StopReceiving ();

			/* Especifico para Windows para que no se cierre automaticamente la ventana */
			Console.Write("\nPress any key to continue... ");
			Console.ReadKey();
		}

		static void Bot_OnMessage (object sender, Telegram.Bot.Args.MessageEventArgs e) {
			// Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
			TelegramBotClient bot = (TelegramBotClient)sender;
			// Bloque debug para la APP
			Console.WriteLine ("Message Received: ");
			Console.WriteLine ("ChatID: " + e.Message.Chat.Id);
			Console.WriteLine ("MessageID: " + e.Message.MessageId);
			Console.WriteLine ("FromID: " + e.Message.From.Id);
			Console.WriteLine ("Message: " + e.Message.Text);
			Console.WriteLine (" ");
			// Devuelve el mensaje que le ha llegado a quien se lo haya mandado
			bot.SendTextMessageAsync (e.Message.Chat.Id, e.Message.Text);
			// Libera el espacio del objeto bot, se crea cada vez que se recibe un mensaje
			bot = null;

		}

		private void loadConfig(nasBot obj) {
			Console.Write ("Reading configuration from files... ");
			obj.botKey = obj.loadSetting ("key");
			Console.Write ("OK");
			Console.WriteLine (" ");
		}

		private void saveSettings(string index, string value) {
			Configuration config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
			config.AppSettings.Settings.Remove (index);
			config.AppSettings.Settings.Add (index, value);
			config.Save (ConfigurationSaveMode.Modified);
		}

		private string loadSetting(string index) {
			return ConfigurationManager.AppSettings[index];
		}
	}
}