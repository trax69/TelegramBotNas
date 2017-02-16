﻿using System;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Helpers;
using Telegram.Bot.Types;

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

		/* Si el token es nulo */
		obj.checkToken (obj.botKey, obj);

		Console.Write ("Starting bot... ");
		// Asignar el valor del token al bot y asignar el bot a una variable
		var bot = new TelegramBotClient (obj.botKey);

		// Ponerle nombre a la consola
		Console.Title = bot.GetMeAsync().Result.Username + " - Listening";

		/* Iniciar Bot*/
		bot.StartReceiving ();
		bot.OnMessage += Bot_OnMessage;
		bot.OnMessageEdited += Bot_OnMessage;

		Console.WriteLine ("OK.");

		/* Especifico para Windows para que no se cierre automaticamente la ventana */
		Console.WriteLine("\nPress ENTER to STOP the bot and EXIT.");
		Console.ReadLine();
		// Parar el bot !
		bot.StopReceiving();
		Environment.Exit(0);
	}

	static void Bot_OnMessage (object sender, Telegram.Bot.Args.MessageEventArgs e) {
		// Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
		var bot = (TelegramBotClient)sender;
		Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
		// Bloque debug para la APP
		Console.WriteLine ("Message Received(" +
			"ID: " + e.Message.MessageId +
			" ChatID: " + e.Message.Chat.Id +
			" FromID: " + e.Message.From.Id +
			") Message: " + e.Message.Text + "\n");

		if (e.Message.Text.StartsWith ("/")) {
			keyboard = makeKeyboard (e.Message.Text);
		} else {
			keyboard = makeKeyboard ("default");
		}
			
		bot.SendTextMessageAsync (e.Message.Chat.Id, e.Message.Text, replyMarkup: keyboard);

		bot = null;
	}

	private static Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup makeKeyboard(string menu) {
		var reply = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
		reply.OneTimeKeyboard = true;
		reply.ResizeKeyboard = true;
		reply.Selective = true;

		switch (menu) {
		case "/start":
			reply.Keyboard = new[] {
				new[] {
					new KeyboardButton("/caca"),
					new KeyboardButton("/culo")
				},
				new[] {
					new KeyboardButton("/pedo"),
					new KeyboardButton("/pis")
				}
			};
			break;
		case "default":
			reply.Keyboard = new[] {
				new[] {
					new KeyboardButton("/start")
				}
			};
			break;
		}

		return reply;
	}

	/* Metodo para comprobar que el token es correcto */
	private void checkToken(string token, nasBot obj) {
		Console.Write ("Checking bot token... ");
		// Probar si el token no es nulo y si contiene ':'
		if (token != null && token.Contains (":")) {
			try {
				TelegramBotClient bot = new TelegramBotClient (token);
				if (bot.TestApiAsync().Result) {
					Console.WriteLine ("OK.");
				}
			} catch (System.ArgumentException) {
				obj.checkToken (null, obj);
			}
		} else {
			Console.WriteLine ("Fail.");
			Console.WriteLine ("Couldn't detect bot Token, please write it now: ");
			string botToken = Console.ReadLine ();
			Console.WriteLine ("Is this your token ? (Y/N): " + botToken);
			// Comprobar que ha escrito la Y
			if (Console.ReadKey ().Key == ConsoleKey.Y) {
				obj.botKey = botToken;
				obj.saveSettings ("key", botToken);
				Console.WriteLine ("Token (" + botToken + ") saved !");
				obj.checkToken (botToken, obj);
			} else {
				obj.checkToken (null, obj);
			}
		}
	}

	private void loadConfig(nasBot obj) {
		Console.Write ("Reading configuration files... ");
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
