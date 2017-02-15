using System;
using System.Configuration;
using Telegram.Bot;

namespace nasBot {
	class nasBot {

		string botKey = "";
		public static void Main(string[] args) {

			/* Crear una instancia del objeto para poder acceder a los metodos privados */
			var obj = new nasBot ();

			/* Recorrer los argumentos pasados a la app */
			foreach (string arg in args) {
				// Si los argumentos contienen un igual EJ: Key=Value
				if (arg.Contains ("=")) {
					// Separa el string en un array dividiendo por el '='
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

			/* Si el token es nulo ( a trax69 le pasa mucho ) */
			if (obj.botKey == null) { 
				Console.WriteLine("Couldn't detect botTokenKey, please insert it now: ");
				obj.botKey = Console.ReadLine();
				Console.WriteLine("Is this your token ?(Y/N): " + obj.botKey);
				if (Console.ReadKey().Key == ConsoleKey.Y) {
					obj.saveSettings("key", obj.botKey);
					Console.WriteLine("Your token has been saved !");
				}
				else { 
					Console.WriteLine("We are sorry to hear that, please start again the app.");
				}
			}

			var bot = new TelegramBotClient (obj.botKey);
			// Ponerle nombre a la consola
			Console.Title = bot.GetMeAsync().Result.Username;
			bot.StartReceiving ();
			bot.OnMessage += Bot_OnMessage;
			bot.OnMessageEdited += Bot_OnMessage;

			/* Especifico para Windows para que no se cierre automaticamente la ventana */
			Console.Write("\nPress ENTER to STOP the bot and EXIT.");
			Console.ReadLine();
			// Parar el bot !
			bot.StopReceiving();
			Environment.Exit(0);
		}

		static void Bot_OnMessage (object sender, Telegram.Bot.Args.MessageEventArgs e) {
			// Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
			var bot = (TelegramBotClient)sender;
			// Bloque debug para la APP
			Console.WriteLine ("Message Received: ");
			Console.WriteLine ("ChatID: " + e.Message.Chat.Id);
			Console.WriteLine ("MessageID: " + e.Message.MessageId);
			Console.WriteLine ("FromID: " + e.Message.From.Id);
			Console.WriteLine ("Message: " + e.Message.Text);
			Console.WriteLine(" ");
			/* ------------------- EJEMPLO DE TECLADO 
			var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(new[] {
				new[] { // Primera fila de opciones
					new Telegram.Bot.Types.KeyboardButton("Primero")
				},
				new[] { // Segunda fila de opciones
					new Telegram.Bot.Types.KeyboardButton("Segundo"),
					new Telegram.Bot.Types.KeyboardButton("Tercero")
				}
			});
			keyboard.OneTimeKeyboard = true;
			keyboard.ResizeKeyboard = true;
			keyboard.Selective = true;
			// Manda mensaje con teclado custom
			bot.SendTextMessageAsync (e.Message.Chat.Id, e.Message.Text, true, true,replyMarkup: keyboard);
			*/

			// Devuelve el mensaje que le ha llegado a quien se lo haya mandado
			bot.SendTextMessageAsync(e.Message.Chat.Id, e.Message.Text);
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