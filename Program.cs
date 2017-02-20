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
	static string botKey;
	static string pW;
	static List<int> authID = new List<int>(); // Variable que guarda las personas autenticadas
	static appConfig conf = new appConfig();
	static appCheck chk = new appCheck ();
	static consoleTweaks text = new consoleTweaks();

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
                    conf.setSetting("key", data[1]);
				}
				else if (data[0].Contains("pW"))
				{
					// Save passWord
                    conf.setSetting("pW",data[1]);
				}
				Environment.Exit(0);
			}
		}

		/* Dar la bienvenida al usuario :-) */
		text.writeWithColor("Welcome to Telegram Bot App !",newLine: true);

        /* Cargar configuración del bot */
        conf.loadConfig();
        botKey = conf.getKey();
        pW = conf.getPW();
        authID = conf.getAuth();

		/* Comprobación del token del bot */
		while (chk.checkToken (botKey) == false) {
			botKey = chk.getToken (); // Si la comprobación sale bien asignar el token
		}
			
		/* Comprobación de la contraseña de autenticación */
		while (chk.checkAuth (pW) == false) {
			pW = chk.getpW (); // Si la comprobación sale bien asignar la contraseña
		}

		text.writeWithColor("Starting bot... ");
		// Asignar el valor del token al bot y asignar el bot a una variable
		var bot = new TelegramBotClient(botKey);

		// Ponerle nombre a la consola
		Console.Title = bot.GetMeAsync().Result.Username + " - Listening";

		/* Iniciar Bot*/
		bot.StartReceiving();

		/* Hacer que cuando se disparen los eventos llamen a un método */
		bot.OnMessage += Bot_OnMessage;
		bot.OnMessageEdited += Bot_OnMessage;

		text.writeWithColor ("Ok. ", ConsoleColor.DarkGreen, true);

		/* Especifico para Windows para que no se cierre automaticamente la ventana */
		Console.BackgroundColor = ConsoleColor.White;
		text.writeWithColor ("\nPress ENTER to STOP the bot and EXIT\n", ConsoleColor.Black, true);

		// Qué la consola no se cierre
		Console.ReadLine();

        // Guardar la lista de autorizados
        conf.setSetting("authID", authID);

		// Parar el bot !
		bot.StopReceiving();

        // Salida sin errores
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
					if (!authID.Contains (e.Message.From.Id)) {
						authID.Add (e.Message.From.Id);
						Console.WriteLine ("UserID: " + e.Message.From.Id + " added to the auth list.");
					} else {
						Console.WriteLine ("UserID: " + e.Message.From.Id + " already in the auth list.");
					}
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
		string texto = "";

		switch (menu) {
			case "/start":
			reply.Keyboard = new[] {
				new[] {
					new KeyboardButton ("/auth")
				}
			};
				
			bot.SendTextMessageAsync(chatID,"<i>Activando sistemas</i> <b>!</b>", replyMarkup: reply, parseMode: ParseMode.Html);
			break;

			case "default":
			reply.Keyboard = new[] {
				new[] {
					new KeyboardButton ("/start")
				}
			};
			texto = "<b>Comando (" + e.Message.Text + ") no reconocido</b>, <i>porfavor vuelve a empezar</i>.";
			bot.SendTextMessageAsync(chatID, texto, replyMarkup: reply, parseMode: ParseMode.Html);
			break;

			case "/server":
				if (authID.Contains (e.Message.From.Id)) {
					reply.Keyboard = new[]
					{
						new[]
						{
							new KeyboardButton("/rebootServer"),
							new KeyboardButton("/shutdownServer")
						},
						new [] {
							new KeyboardButton("/authMenu")
						}
					};
					
					texto = "<i>Sección</i> <b>Server</b>";
				} else {
					reply.Keyboard = new[]
					{
						new[]
						{
							new KeyboardButton("/auth"),
						}
					};

					texto = "<i>No tienes <b>acceso</b> a este comando</i>";
				}

				bot.SendTextMessageAsync(chatID, texto, replyMarkup: reply, parseMode: ParseMode.Html);
			break;

			case "/torrent":
				if (authID.Contains (e.Message.From.Id)) {
					reply.Keyboard = new[] {
						new[] {
							new KeyboardButton ("/uploadTorrent"),
							new KeyboardButton ("/listTorrent")
						},
						new[] {
							new KeyboardButton ("/deleteTorrent"),
							new KeyboardButton ("/checkTorrent")
						},
						new [] {
							new KeyboardButton ("/authMenu")
						}
					};

					texto = "<i>Sección</i> <b>Torrent</b>";
				} else {
					reply.Keyboard = new[]
					{
						new[]
						{
							new KeyboardButton("/auth"),
						}
					};

					texto = "<i>No tienes <b>acceso</b> a este comando</i>";
				}

				bot.SendTextMessageAsync(chatID, texto, replyMarkup: reply, parseMode: ParseMode.Html);
			break;

			case "/auth":
				isAuth = true;
				bot.SendTextMessageAsync(chatID, "<i>Porfavor introduce la contraseña</i>", parseMode: ParseMode.Html);
			break;

			case "/authMenu":
				if (authID.Contains (e.Message.From.Id)) {
					reply.Keyboard = new[] {
						new[] {
							new KeyboardButton ("/server")
						},
						new[] {
							new KeyboardButton ("/torrent"),
						}
					};

					texto = "<i>Menú con privilegios</i>";
				} else {
					reply.Keyboard = new[]
					{
						new[]
						{
							new KeyboardButton("/auth"),
						}
					};

					texto = "<i>No tienes <b>acceso</b> a este comando</i>";
				}

				bot.SendTextMessageAsync(chatID, texto, replyMarkup: reply, parseMode: ParseMode.Html);
			break;

			case "authOk":
				reply.Keyboard = new[] {
					new[] {
						new KeyboardButton ("/server")
					},
					new[] {
						new KeyboardButton ("/torrent"),
					}
				};
				isAuth = false;
				bot.SendTextMessageAsync(chatID, "<i>Estás logueado</i>", replyMarkup: reply, parseMode: ParseMode.Html);
			break;
				
			case "authFail":
				isAuth = false;
				bot.SendTextMessageAsync(chatID, "<i>Contraseña incorrecta NO.OB</i>", parseMode: ParseMode.Html);
			break;
		}
	}
}