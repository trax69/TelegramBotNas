using System;
using System.Collections.Generic;
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
	static List<long> authID = new List<long>(); // Variable que guarda las personas autenticadas
	static appConfig conf = new appConfig();
	static appCheck chk = new appCheck ();

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
					// Save Token
					conf.saveSettings(data[1]);
					Console.WriteLine("The key has been successfully saved !");
				}
				else if (data[0].Contains("pW"))
				{
					// Save passWord
					conf.saveSettings(data[1]);
					Console.WriteLine("The password has been successfully saved !");
				}
				Environment.Exit(0);
			}
		}

		/* Dar la bienvenida al usuario :-) */
		Console.WriteLine("Welcome to Telegram Bot App !");

		/* Cargar configuración del bot */
		conf.loadConfig ();

		/* Comprobación del token del bot */
		chk.checkToken(botKey);

		/* Comprobación de la contraseña de autenticación */
		chk.checkAuth(pW);

		Console.Write("Starting bot... ");
		// Asignar el valor del token al bot y asignar el bot a una variable
		var bot = new TelegramBotClient(botKey);

		// Ponerle nombre a la consola
		Console.Title = bot.GetMeAsync().Result.Username + " - Listening";

		/* Iniciar Bot*/
		bot.StartReceiving();

		/* Hacer que cuando se disparen los eventos llamen a un método */
		bot.OnMessage += Bot_OnMessage;
		bot.OnMessageEdited += Bot_OnMessage;

		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("OK.");
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

	static bool isAuth = false; // Variable que guarda si el usuario está en proceso de autenticación
	static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
	{
		// Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
		var bot = (TelegramBotClient)sender;
		// Bloque debug para la APP
		Console.WriteLine("Message(" +
			"ID: " + e.Message.MessageId +
			" ChatID: " + e.Message.Chat.Id +
			" FromID: " + e.Message.From.Id +
			"): " + e.Message.Text);

		if (e.Message.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			sendWithKeyboard(e.Message.Text, e, bot);
		}
		else
		{
			if (isAuth)
			{
				if (e.Message.Text == pW)
				{
					authID.Add(e.Message.From.Id);
					Console.WriteLine ("UserID: " + e.Message.From.Id + " added to the auth list.");
					sendWithKeyboard("authOk", e, bot);
				}
				else
				{
					Console.WriteLine ("User(FN: " + e.Message.From.FirstName +
						" LN: " + e.Message.From.LastName + 
						" ID: " + e.Message.From.Id + ") tryed to auth and fail.");
					sendWithKeyboard("authFail", e, bot);
				}
			}
			else
			{
				sendWithKeyboard("default", e, bot);
			}
		}

		bot = null;
	}

	private static void sendWithKeyboard(string menu, MessageEventArgs e, TelegramBotClient bot)
	{
		var reply = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
		reply.OneTimeKeyboard = true;
		reply.ResizeKeyboard = true;
		reply.Selective = true;

		long chatID = e.Message.Chat.Id;


		switch (menu)
		{
			case "/start":
				reply.Keyboard = new[]
				{
					new[]
					{
						new KeyboardButton("/server"),
						new KeyboardButton("/auth"),
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

			case "/server":
				reply.Keyboard = new[]
				{
					new[]
					{
						new KeyboardButton("/startServer"),
						new KeyboardButton("/stopServer"),
						new KeyboardButton("/pingServer"),
						new KeyboardButton("/deleteServer")
					}
				};

				bot.SendTextMessageAsync(chatID, "<i>Sección</i> <b>Server</b>", replyMarkup: reply, parseMode: ParseMode.Html);
				break;

			case "/auth":
				isAuth = true;
				bot.SendTextMessageAsync(chatID, "<i>Porfavor introduce la contraseña</i>", parseMode: ParseMode.Html);
				break;

			case "authOk":
				isAuth = false;
				bot.SendTextMessageAsync(chatID, "<i>Estás logueado</i>", parseMode: ParseMode.Html);
				break;
				
			case "authFail":
				isAuth = false;
				bot.SendTextMessageAsync(chatID, "<i>Contraseña incorrecta NO.OB</i>", parseMode: ParseMode.Html);
				break;
		}
	}
}