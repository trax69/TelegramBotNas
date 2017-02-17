﻿using System;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class nasBot {

	static string botKey; // Variable que guarda el token del bot
	static string pW; // Variable que guarda la contraseña de comandos
	static long[] authID; // Variable que guarda las personas autenticadas

	public static void Main(string[] args) 
	{
		/* Limpiar la consola */
		Console.Clear();

		/* Recorrer los argumentos pasados a la app */
		foreach (string arg in args) 
		{
			// Si los argumentos contienen un igual EJ: Key=Value
			if (arg.Contains("key=") || arg.Contains("pW="))
			{
				// Separa el string en un array dividiendo por el '='
				string[] data = arg.Split((char)'=');
				// Si la primera parte del array contiene 'key', asignamos en los ajustes
				if (data[0].Contains("key"))
				{
					saveSettings("key", data[1]);
					Console.WriteLine("The key has been successfully saved !");
				}
				else if (data[0].Contains("pW")) 
				{
					saveSettings("pW", data[1]);
					Console.WriteLine("The password has been successfully saved !");
				} 
				Environment.Exit(0);
			}
		}

		/* Dar la bienvenida al usuario :-) */
		Console.WriteLine("Welcome to Telegram Bot App !");

		/* Cargar configuración del bot */
		loadConfig();

		/* Comprobación del token del bot */
		checkToken (botKey);

		/* Comprobación de la contraseña de autenticación */
		checkAuth(pW);

		Console.Write ("Starting bot... ");
		// Asignar el valor del token al bot y asignar el bot a una variable
		var bot = new TelegramBotClient (botKey);

		// Ponerle nombre a la consola
		Console.Title = bot.GetMeAsync().Result.Username + " - Listening";

		/* Iniciar Bot*/
		bot.StartReceiving ();

		/* Hacer que cuando se disparen los eventos llamen a un método */
		bot.OnMessage += Bot_OnMessage;
		bot.OnMessageEdited += Bot_OnMessage;

		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine ("OK.");
		Console.ResetColor();

		/* Especifico para Windows para que no se cierre automaticamente la ventana */
		Console.BackgroundColor = ConsoleColor.White;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.WriteLine("\nPress ENTER to STOP the bot and EXIT\n");
		Console.ResetColor();
		// Qué la consola no se cierre
		Console.ReadLine();
		// Parar el bot !
		bot.StopReceiving();
		Environment.Exit(0);
	}

	static bool isAuth; // Variable que guarda si el usuario está en proceso de autenticación

	static void Bot_OnMessage (object sender, Telegram.Bot.Args.MessageEventArgs e) 
	{
		// Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
		var bot = (TelegramBotClient)sender;
		// Bloque debug para la APP
		Console.WriteLine ("Message(" +
			"ID: " + e.Message.MessageId +
			" ChatID: " + e.Message.Chat.Id +
			" FromID: " + e.Message.From.Id +
			"): " + e.Message.Text);

		if (e.Message.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase)) 
		{
			sendWithKeyboard (e.Message.Text, e.Message.Chat.Id ,bot);
		} 
		else 
		{
			sendWithKeyboard("default", e.Message.Chat.Id, bot);
		}

		bot = null;
	}

	private static void sendWithKeyboard(string menu, long chatID, TelegramBotClient bot) 
	{
		var reply = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
		reply.OneTimeKeyboard = true;
		reply.ResizeKeyboard = true;
		reply.Selective = true;

		switch (menu) {
		case "/start":
			reply.Keyboard = new[] 
			{
				new[] 
				{
					new KeyboardButton("/caca"),
					new KeyboardButton("/culo")
				},
				new[] 
				{
					new KeyboardButton("/pedo"),
					new KeyboardButton("/pis")
				}
			};

			bot.SendTextMessageAsync(chatID, "<i>Activando sistemas </i><b>!</b>", replyMarkup: reply, parseMode: ParseMode.Html);
			break;
		case "default":
			reply.Keyboard = new[] 
			{
				new[] 
				{
					new KeyboardButton("/start")
				}
			};

			bot.SendTextMessageAsync(chatID, "<b>Comando no reconocido</b>, <i>porfavor vuelve a empezar</i>.", replyMarkup: reply, parseMode: ParseMode.Html);
			break;
		}
	}


	/* Metodo para comprobar que el token es correcto */
	private static void checkToken(string token) 
	{
		Console.Write ("Checking bot token... ");
		// Probar si el token no es nulo y si contiene ':'
		if (token != null && token.Contains (":")) 
		{
			try 
			{
				var bot = new TelegramBotClient (token);
				if (bot.TestApiAsync().Result) 
				{
					// Añadidos colorines al OK.
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine("OK.");
					Console.ResetColor();
				}
			} 
			catch (System.ArgumentException) 
			{
				checkToken (null);
			}
		} 
		else 
		{
			// Añadidos colorines al Fail.
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Fail.");
			Console.ResetColor();

			Console.WriteLine ("Couldn't detect bot Token, please write it now: ");
			botKey = Console.ReadLine ();
			Console.Write ("Is the token correct ? (Y/N): ");
			// Comprobar que ha escrito la Y
			if (Console.ReadKey ().Key == ConsoleKey.Y) {
				Console.WriteLine(" ");
				saveSettings ("key", botKey);
				Console.WriteLine ("Token (" + botKey + ") saved !");
				checkToken (botKey);
			} 
			else 
			{
				checkToken (null);
			}
		}
	}

	private static void checkAuth(string passWord) {
		Console.Write("Checking auth password... ");
		if (passWord == null)
		{
			// Añadidos colorines al Fail.
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Fail.");
			Console.ResetColor();

			Console.WriteLine("Couldn't detect auth password, please write it now: ");
			pW = Console.ReadLine();
			Console.Write("Is the password correct ? (Y/N): ");
			if (Console.ReadKey().Key == ConsoleKey.Y)
			{
				Console.WriteLine(" ");
				saveSettings("pW", pW);
				Console.WriteLine("Password (" + pW + ") saved !");
				checkAuth(pW);
			}
			else
			{
				checkAuth(null);
			}
		}
		else 
		{
			// Añadidos colorines al OK.
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("OK.");
			Console.ResetColor();
		}
	}

	private static void loadConfig() {
		Console.Write("Reading configuration files... ");
		// Asignación de variables
		botKey = loadSetting("key");
		pW = loadSetting("pW");
		// Añadidos colorines al OK
		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("OK.");
		Console.ResetColor();
	}

	private static void saveSettings(string index, string value) 
	{
		Configuration config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
		config.AppSettings.Settings.Remove (index);
		config.AppSettings.Settings.Add (index, value);
		config.Save (ConfigurationSaveMode.Modified);
	}

	private static string loadSetting(string index) 
	{
		return ConfigurationManager.AppSettings[index];
	}
}
