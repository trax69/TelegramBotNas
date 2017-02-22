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
    static List<int> authID; // Variable que guarda las personas autenticadas
    static appConfig conf;  // Variable para gestionar la configuración
    static appCheck chk;    // Variable para gestionar los chequeos internos
    static consoleTweaks text;  // Variable para gestionar la consola

	public static void Main(string[] args)
	{
        /* Iniciando Variables */
        conf = new appConfig();
        chk = new appCheck ();
        text = new consoleTweaks();
        authID = new List<int> ();

		/* Limpiar la consola */
		Console.Clear();

		/* Recorrer los argumentos pasados a la app */
		foreach (string arg in args)
		{
			// Si los argumentos contienen un igual EJ: Key=Value
			if (arg.Contains("key=") || arg.Contains("pW="))
			{
                conf.loadConfig ();
				// Separa el string en un array dividiendo por el '='
				string[] data = arg.Split((char)'=');
				// Si la primera parte del array contiene 'key', asignamos en los ajustes
				if (data[0].Contains("key"))
				{
					// Save Token
                    conf.setKey(data[1]);
				}
				else if (data[0].Contains("pW"))
				{
					// Save passWord
                    conf.setPW(data[1]);
				}

                conf.save (true);
				Environment.Exit(0);
			}
		}

		/* Dar la bienvenida al usuario :-) */
        text.writeWithColor("Welcome to Telegram Bot App !\n", ConsoleColor.Blue, newLine: true);

        /* Cargar configuración del bot */
        conf.loadConfig();

        /* Asignar Variables */
        botKey = conf.getKey();
        pW = conf.getPW();
        authID = conf.getAuth();
        text.writeWithColor ("\n");

		/* Comprobación del token del bot */
        if (chk.checkToken (botKey)) {
            botKey = chk.botToken;
        }
	    
		/* Comprobación de la contraseña de autenticación */
        if (chk.checkAuth (pW)) {
            pW = chk.authPass;
        }

		text.writeWithColor("\nStarting bot... ");
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
        conf.loadConfig();
        conf.setAuth(authID);
        conf.save (true);   // Guardar los ajustes

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
		// Bloque debug para la consola
        text.writeWithColor ("Message (ID: ");
        text.writeWithColor (e.Message.MessageId.ToString(), ConsoleColor.Blue);
        text.writeWithColor (" ChatID: ");
        text.writeWithColor (e.Message.Chat.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor (" FromID: ");
        text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor (" ): ");
        text.writeWithColor (e.Message.Text, ConsoleColor.DarkBlue, true);

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
                    text.writeWithColor ("ID's: " + authID.Count);
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