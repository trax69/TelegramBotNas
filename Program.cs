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
        text.writeWithColor ("): ");
        text.writeWithColor (e.Message.Text, ConsoleColor.DarkBlue, true);

		if (e.Message.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			sendWithKeyboard(e.Message.Text, e, bot);
		} else {
			if (isAuth)
			{
				if (e.Message.Text == pW)
				{
					if (!authID.Contains (e.Message.From.Id)) {
						authID.Add (e.Message.From.Id);
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
		}

		bot = null;
	}

    private static bool isUserAuth(int userID) {
        bool _auth = false; // Dar por sentado que no está autorizado

        if (authID.Contains(userID)) {
            _auth = true;
        }

        return _auth;
    }

	private static void sendWithKeyboard(string menu, MessageEventArgs e, TelegramBotClient bot)
	{
        var reply = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
		reply.OneTimeKeyboard = true;
		reply.ResizeKeyboard = true;
		reply.Selective = true;

		long chatID = e.Message.Chat.Id;
        int userID = e.Message.From.Id;

		switch (menu) {

			case "/start":
            if (!isUserAuth(userID)) {
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

			case "default":
    			reply.Keyboard = new[] {
    				new[] {
    					new KeyboardButton ("/start")
    				}
    			};
    			string texto = "<b>Comando (<i>" + e.Message.Text + "</i>) no reconocido</b>, <i>porfavor vuelve a empezar</i>.";
    			bot.SendTextMessageAsync(chatID, texto, replyMarkup: reply, parseMode: ParseMode.Html);
			break;

            case "/server":
                if (isUserAuth(e.Message.From.Id)) {
                    reply.Keyboard = new[] {
                        new[] {
                            new KeyboardButton ("/rebootServer"),
                            new KeyboardButton ("/shutdownServer")
                        },
                        new [] {
                            new KeyboardButton("/unAuth")    
                        },
                        new [] {
                            new KeyboardButton ("/authMenu")
                        }
                    };
                    bot.SendTextMessageAsync (chatID, "<i>Sección</i> <b>Server</b>", replyMarkup: reply, parseMode: ParseMode.Html);
                } else {
                    sendWithKeyboard ("noAuth", e, bot);
                }
			break;

			case "/torrent":
                if (isUserAuth(e.Message.From.Id)) {
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
                    bot.SendTextMessageAsync(chatID, "<i>Sección</i> <b>Torrent</b>", replyMarkup: reply, parseMode: ParseMode.Html);
				} else {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
				}
			break;

			case "/auth":
				isAuth = true;
				bot.SendTextMessageAsync(chatID, "<i>Porfavor introduce la contraseña</i>", parseMode: ParseMode.Html);
			break;

            case "/unAuth":
                if (isUserAuth (e.Message.From.Id)) {
                    authID.Remove (e.Message.From.Id);
                    // Debug for console
                    text.writeWithColor ("UserID: ", ConsoleColor.DarkYellow);
                    text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.DarkRed);
                    text.writeWithColor (" removed from the auth list. ", ConsoleColor.DarkYellow, true);
                };

                bot.SendTextMessageAsync (chatID, "<i>Ya <b>no</b> estás autorizado en este bot.</i>",parseMode: ParseMode.Html);
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
                    bot.SendTextMessageAsync(chatID, "<i>Menú con privilegios</i>", replyMarkup: reply, parseMode: ParseMode.Html);
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
				isAuth = false;
				bot.SendTextMessageAsync(chatID, "<i>Estás logueado</i>", replyMarkup: reply, parseMode: ParseMode.Html);
			break;
				
			case "authFail":
				isAuth = false;
				bot.SendTextMessageAsync(chatID, "<i>Contraseña incorrecta NO.OB</i>", parseMode: ParseMode.Html);
			break;

            case "noAuth":
                reply.Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("/auth"),
                    }
                };

                texto = "<i>No tienes <b>acceso</b> a este comando</i>";
            break;
		}
	}
}