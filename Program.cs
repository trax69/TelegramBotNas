using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using botSettings;
using botSettings.Checks;
using botSettings.Enums;

class nasBot {
	static string botKey;
	static string passWd;
    static botSetting conf;
    static consoleTweaks text;  // Variable para gestionar la consola

	public static void Main(string[] args)
	{
        string filePath = AppDomain.CurrentDomain.BaseDirectory;
        string fileName = "config.json";

        /* Iniciando Variables */
        conf = new botSetting (filePath + fileName);
        filePath = fileName = null;
        text = new consoleTweaks();

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
                    conf.botToken = data [1];
				}
				else if (data[0].Contains("pW"))
				{
					// Save passWord
                    conf.passWord = data [1];
				}

                text.writeWithColor ("");
                conf.saveConfig (); // Guardar la config
				Environment.Exit(0);
			}
		}

		/* Titulo de la APP ! :-) */
        text.Title ("Welcome to Telegram Bot App !", char.Parse("#"), ConsoleColor.Blue);

        /* Cargar configuración del bot */
        conf.loadConfig();

        /* Asignar Variables */
        botKey = conf.botToken;
        passWd = conf.passWord;

        if (string.IsNullOrEmpty (botKey) || string.IsNullOrWhiteSpace (botKey))
        {
            botChecks chk = new botChecks ();
            while (chk.checkToken (botKey) == false)
            {
                botKey = createToken ();
            }
            conf.botToken = botKey;
            chk = null;
            Console.WriteLine ();
        }

        if (string.IsNullOrEmpty (passWd) || string.IsNullOrWhiteSpace (passWd))
        {
            botChecks chk = new botChecks ();
            while (chk.checkPW (passWd) == false)
            {
                passWd = createPassword ();
            }
            conf.passWord = passWd;
            chk = null;
            Console.WriteLine ();
        }

        text.writeWithColor ("Using Token: ");
        text.writeWithColor (conf.botToken, ConsoleColor.Magenta, true);
        text.writeWithColor ("Using Password: ");
        text.writeWithColor (conf.passWord, ConsoleColor.Magenta, true);
        Console.WriteLine ();

		text.writeWithColor("Starting bot... ");
		// Asignar el valor del token al bot y asignar el bot a una variable
		var bot = new TelegramBotClient(botKey);

		// Ponerle nombre a la consola
        Console.Title = bot.GetMeAsync().Result.Username + " - Listening";

		/* Iniciar Bot*/
		bot.StartReceiving();

		/* Hacer que cuando se disparen los eventos llamen a un método */
		bot.OnMessage += Bot_OnMessage;
        bot.OnInlineQuery += Bot_OnInlineQuery;
        bot.OnInlineResultChosen += Bot_OnInlineResultChosen;

		text.writeWithColor ("Ok. ", ConsoleColor.DarkGreen, true);

		/* Especifico para Windows para que no se cierre automaticamente la ventana */
		Console.BackgroundColor = ConsoleColor.White;
        text.writeWithColor ("Write QUIT to exit execution\n", ConsoleColor.Black, true);

		// Qué la consola no se cierre
        string consoleMenu;
        // Bucle comandos de la consola
        while ((consoleMenu = Console.ReadLine ().ToLower()) != "quit") {
            consoleMenu = "Comando Recibido: " + consoleMenu;
            Console.WriteLine (consoleMenu.PadLeft(consoleMenu.Length + 1));
        }

        // Guardar la configuración con la lista de autorizados
        conf.saveConfig ();

		// Parar el bot !
		bot.StopReceiving();

        // Salida sin errores
		Environment.Exit(0);
	}

    /// <summary>
    /// Creates the token.
    /// </summary>
    /// <returns>The token.</returns>
    public static string createToken()
    {
        string rToken = "";
        Console.WriteLine ();
        text.writeWithColor ("Unable to find usable token, please write it now: ", newLine: true);
        rToken = Console.ReadLine ();
        text.writeWithColor ("Is the Token correct ? (Y/N): ");
        if (!(Console.ReadKey ().Key == ConsoleKey.Y)) 
        {
            createToken ();
        }
        return rToken;
    }

    /// <summary>
    /// Creates the password.
    /// </summary>
    /// <returns>The password.</returns>
    public static string createPassword()
    {
        string rPW = "";
        Console.WriteLine ();
        text.writeWithColor ("Unable to find usable password, please write it now: ", newLine: true);
        rPW = Console.ReadLine ();
        text.writeWithColor ("Is the password correct ? (Y/N): ");
        if (!(Console.ReadKey ().Key == ConsoleKey.Y)) 
        {
            createPassword();
        }
        return rPW;
    }

    static void Bot_OnInlineQuery (object sender, InlineQueryEventArgs e)
    {
        text.writeWithColor ("InlineQuery (ID: ");
        text.writeWithColor (e.InlineQuery.Id, ConsoleColor.Blue);
        text.writeWithColor (" FromID: ");
        text.writeWithColor (e.InlineQuery.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor ("): ");
        text.writeWithColor (e.InlineQuery.Query, ConsoleColor.DarkBlue, true);
    }

    static void Bot_OnInlineResultChosen (object sender, ChosenInlineResultEventArgs e)
    {
        text.writeWithColor ("InlineResult (InlineMsgID: ");
        text.writeWithColor (e.ChosenInlineResult.InlineMessageId, ConsoleColor.Blue);
        text.writeWithColor (" ResultID: ");
        text.writeWithColor (e.ChosenInlineResult.ResultId, ConsoleColor.Blue);
        text.writeWithColor (" FromID: ");
        text.writeWithColor (e.ChosenInlineResult.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor (" ForQuery: ");
        text.writeWithColor (e.ChosenInlineResult.Query, ConsoleColor.Blue);
        text.writeWithColor (" )", newLine: true);
    }

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
        text.writeWithColor ("): ");
        text.writeWithColor (e.Message.Text, ConsoleColor.DarkBlue, true);

        int listIndex = conf.getIndexAuthList (e.Message.From.Id);

        if (listIndex == -1) {
            conf.addUser (e.Message.From.Id, e.Message.From.FirstName,
                e.Message.From.LastName, e.Message.From.Username);
        }

		if (e.Message.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			sendWithKeyboard(e.Message.Text, e, bot);
		} 
        else
        {
            if (conf.getIsUserInAuthProcess (listIndex)) {
                if (e.Message.Text == conf.passWord) {
                    conf.setUserIsAuth (listIndex, true);
                    sendWithKeyboard ("authOk", e, bot);
                } else {
                    sendWithKeyboard ("authFail", e, bot);
                }
            } else {
                sendWithKeyboard ("caca", e, bot);
            }
            /*
            Console.WriteLine (conf.ToString ());
            if (e.Message.From.Id)
			{
				if (e.Message.Text == pW)
				{
                    if (!conf.isUserInAuthList (e.Message.From.Id)) {
                        conf.addUser (e.Message.From.Id);
                        // Debug info for console
                        text.writeWithColor ("UserID: ", ConsoleColor.DarkYellow);
                        text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.DarkGreen);
                        text.writeWithColor (" added to the auth list. ", ConsoleColor.DarkYellow, true);
					} else {
                        // Debug info for console
                        text.writeWithColor ("UserID: ", ConsoleColor.DarkYellow);
                        text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.DarkRed);
                        text.writeWithColor (" already in the auth list. ", ConsoleColor.DarkYellow, true);
					}
					sendWithKeyboard("authOk", e, bot);
				} else {
                    // Debug info for console
                    if (!(string.IsNullOrEmpty (e.Message.From.FirstName) || string.IsNullOrWhiteSpace (e.Message.From.FirstName)))
                    {
                        text.writeWithColor (e.Message.From.FirstName + " ", ConsoleColor.DarkYellow);
                    }
                    if (!(string.IsNullOrEmpty (e.Message.From.LastName) || string.IsNullOrWhiteSpace (e.Message.From.LastName)))
                    {
                        text.writeWithColor (e.Message.From.LastName + " ", ConsoleColor.DarkYellow);
                    }
                    text.writeWithColor ("ID: ", ConsoleColor.DarkCyan);
                    text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.DarkYellow);
                    text.writeWithColor (" tryed to auth and fail.", ConsoleColor.DarkCyan, true);

                    // Send message to client to inform of failure
					sendWithKeyboard("authFail", e, bot);
				}
			}
			else
			{
				sendWithKeyboard("default", e, bot);
			}
   */         
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
        int listIndex = conf.getIndexAuthList (e.Message.From.Id);
        bool isAuth = conf.getIsUserAuth (listIndex);

        switch (menu) {
			case "/start":
                if (!isAuth) {
        			reply.Keyboard = new[] {
        				new[] {
        					new KeyboardButton ("/auth")
        				}
                    };
                    bot.SendTextMessageAsync(chatID,"<i>Activando sistemas</i> <b>!</b>", replyMarkup: reply, parseMode: ParseMode.Html);
                } else {
                    sendWithKeyboard ("/authMenu", e, bot); // Si está autenticado redirigir al menu de autenticados
                }
            break;

            case "/server":
                if (!isAuth) {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                } else {
                    reply.Keyboard = new[] {
                        new[] {
                            new KeyboardButton ("/rebootServer")
                        },
                        new [] {
                            new KeyboardButton("/unAuth")    
                        },
                        new [] {
                            new KeyboardButton ("/authMenu")
                        }
                    };
                    bot.SendTextMessageAsync (chatID, "<i>Sección</i> <b>Server</b>", replyMarkup: reply, parseMode: ParseMode.Html);
                }
			break;

            case "/rebootServer":
                if (!isAuth)
                {
                    sendWithKeyboard("noAuth", e, bot); // Ir a la sección de no autorización
                }
                else
                {
                    string rebootTime = "10";   // Tiempo de reinicio en segundos
                    if (botChecks.isLinux())
                    {
                        Process.Start("shutdown", "-r -t " + rebootTime);
                    }
                    else
                    {
                        Process.Start("shutdown.exe", "-r -t " + rebootTime);
                    }
                    bot.SendTextMessageAsync(chatID, "El <b>sistema</b> se va a reiniciar en <i>" + rebootTime + "</i> segundos", parseMode: ParseMode.Html);
                }

            break;

			case "/torrent":
                if (!isAuth) {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                } else {
					reply.Keyboard = new[] {
						new[] {
							new KeyboardButton ("/uploadTorrent")
						},
						new[] {
                            new KeyboardButton ("/listTorrent"),
							new KeyboardButton ("/deleteTorrent")
						},
						new [] {
							new KeyboardButton ("/authMenu")
						}
					};
                    bot.SendTextMessageAsync(chatID, "<i>Sección</i> <b>Torrent</b>", replyMarkup: reply, parseMode: ParseMode.Html);
				}
			break;

            case "/auth":
                if (!isAuth) { 
                    conf.setUserInAuthProcess (listIndex, true);
                    bot.SendTextMessageAsync (chatID, "<i>Porfavor introduce la contraseña</i>", parseMode: ParseMode.Html);
                } else {
                    sendWithKeyboard ("/authMenu", e, bot);
                }
            break;

            case "/unAuth":
                if (!isAuth) {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                } else {
                    conf.setUserIsAuth (listIndex, false);
                    // Debug for console
                    text.writeWithColor ("UserID: ", ConsoleColor.DarkYellow);
                    text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.DarkRed);
                    text.writeWithColor (" removed from the auth list. ", ConsoleColor.DarkYellow, true);
                    bot.SendTextMessageAsync (chatID, "<i>Ya</i> <b>no</b> <i>tienes autorización en este bot</i>.", parseMode: ParseMode.Html);
                };
                
            break;

			case "/authMenu":
                if (isAuth) {
                    reply.Keyboard = new [] {
                            new[] {
                                new KeyboardButton ("/server")
                            },
                            new[] {
                                new KeyboardButton ("/torrent"),
                            }
                        };
                    bot.SendTextMessageAsync (chatID, "<i>Menú con privilegios</i>", replyMarkup: reply, parseMode: ParseMode.Html);
                } else {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                }
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
                conf.setUserInAuthProcess (listIndex, false);
                conf.setUserIsAuth (listIndex, true);
				bot.SendTextMessageAsync(chatID, "<i>Estás logueado</i>", replyMarkup: reply, parseMode: ParseMode.Html);
			break;
				
            case "authFail":
                int intentos = conf.getUserTrys (listIndex);
                DateTime banUntil = conf.getBanUntil (listIndex);
                TimeSpan baneado = banUntil - DateTime.Now;
                if (intentos >= 0 && (baneado.Seconds <= 0)) {
                    intentos = intentos - 1;
                    conf.setUserTrys (listIndex, intentos);
                    bot.SendTextMessageAsync(chatID, "<i>Contraseña incorrecta NO.OB</i> Te quedan " + intentos + " intentos.", parseMode: ParseMode.Html);
                } else if(baneado.Seconds > 0) {
                    bot.SendTextMessageAsync(chatID, "<i>No te quedan intentos</i> Prueba de nuevo en " + baneado.Seconds + " segundos", parseMode: ParseMode.Html);
                }
                sendWithKeyboard("/auth", e, bot);
            break;

            case "noAuth":
                reply.Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("/auth"),
                    }
                };
                bot.SendTextMessageAsync (chatID, "<i>No tienes</i> <b>acceso</b> <i>a este comando</i>", replyMarkup: reply, parseMode: ParseMode.Html);
            break;

            default:
                reply.Keyboard = new [] {
                        new[] {
                            new KeyboardButton ("/start")
                        }
                    };
                bot.SendTextMessageAsync (chatID, "Comando <b>no</b> reconocido", replyMarkup: reply, parseMode: ParseMode.Html);
            break;
		}
	}
}