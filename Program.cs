using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

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
		bot.OnMessage += BotOnMessageReceived;
        bot.OnInlineQuery += BotOnInlineQueryReceived;
        bot.OnInlineResultChosen += BotOnInlineResultChosen;
        bot.OnCallbackQuery += BotOnCallbackQueryReceived;

		text.writeWithColor ("Ok. ", ConsoleColor.DarkGreen, true);

		/* Especifico para Windows para que no se cierre automaticamente la ventana */
		Console.BackgroundColor = ConsoleColor.White;
        text.writeWithColor ("Write QUIT to exit execution\n", ConsoleColor.Black, true);

		// Qué la consola no se cierre
        string consoleMenu;
        // Bucle comandos de la consola
        while ((consoleMenu = Console.ReadLine ().ToLower()) != "quit") 
        {
            switch (consoleMenu)
            {
                case "unBan":
                    break;
                default:
                    text.writeWithColor("Command " + consoleMenu + " not recognized", newLine: true);
                    break;
            }
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

    static async void BotOnInlineQueryReceived (object sender, InlineQueryEventArgs e)
    {
        var bot = (TelegramBotClient)sender;
        text.writeWithColor ("InlineQuery (ID: ");
        text.writeWithColor (e.InlineQuery.Id, ConsoleColor.Blue);
        text.writeWithColor (" FromID: ");
        text.writeWithColor (e.InlineQuery.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor ("): ");
        text.writeWithColor (e.InlineQuery.Query, ConsoleColor.DarkBlue, true);

        InlineQueryResult[] results = {
            new InlineQueryResultContact
            {
                Id = "1",
                FirstName = "C. M.",
                LastName = "B.",
                PhoneNumber = "+34 666777888",
                InputMessageContent = new InputContactMessageContent
                {
                    FirstName = "Cash Monkeys",
                    LastName = "Big",
                    PhoneNumber = "+34 666777888"
                }
            }
        };

        await bot.AnswerInlineQueryAsync(e.InlineQuery.Id, results, 0, true);
    }

    static void BotOnInlineResultChosen (object sender, ChosenInlineResultEventArgs e)
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

    private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
    {
        // AQUI LLEGAN TODAS LAS ORDENES DE LOS BOTONES INLINE (en chat privado) LA OPCIÓN ELEGIDA SE VÉ A TRAVÉS DE e.CallbackQuery.Data
        // PARA CAMBIAR EL MENSAJE SE USA bot.EditMessageCaptionAsync(chatID, MsgID, "Nuevo Mensaje", replyMarkup);
        // PARA CAMBIAR LAS RESPUESTAS (Botones Inline) HAY QUE CAMBIAR EL replyMarkup DEL METODO ANTERIOR
        // PARA COJER EL ID DEL ULTIMO MENSAJE A ESE USUARIO USARIO CON conf.userPropertys(listIndex).lastMsgID;
        var bot = (TelegramBotClient)sender;
        await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Received: {e.CallbackQuery.Data}");
    }

	static async void BotOnMessageReceived(object sender, MessageEventArgs e)
	{
        // Si no es un mensaje de texto pasar en moto de el qué sino da error !
        if (e.Message.Type != MessageType.TextMessage) return;

		// Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
		var bot = (TelegramBotClient)sender;
		// Bloque debug para la consola
        text.writeWithColor ("Message (ID:");
        text.writeWithColor (e.Message.MessageId.ToString(), ConsoleColor.Blue);
        text.writeWithColor (" ChatID:");
        text.writeWithColor (e.Message.Chat.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor (" FromID:");
        text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor ("): ");
        text.writeWithColor(e.Message.Text, ConsoleColor.DarkCyan, true);

        int listIndex = conf.getIndexAuthList (e.Message.From.Id);

        if (listIndex == -1) 
        {
            conf.addUser (e.Message.From.Id, e.Message.From.FirstName, e.Message.From.LastName,
                            e.Message.From.Username, nrTry: (int)2);
        }

        listIndex = conf.getIndexAuthList(e.Message.From.Id);   // Poner el listIndex bien después de añadir a la lista

        string messageToSend = "Default Text";
        if (e.Message.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        {
            if (e.Message.Text.Contains("/start"))
            {
                if (conf.userPropertys(listIndex).isAuth)
                {
                    messageToSend = "You are an authenticated user\nYou can use all bot settings.";
                }
                else
                {
                    messageToSend = "Systems activated !\nPlease login with the command\n/login YourPassword";
                }
            }
            #region userLogin
            else if (e.Message.Text.Contains("/login "))
            {
                if (conf.userPropertys(listIndex).isAuth) return;   // Si el usuario ya está autorizado pasar en moto de que ha puesto login ... pobre tonto.
                string[] pwContainer = e.Message.Text.Split(char.Parse(" "));
                string userPassword = pwContainer[1];
                pwContainer = null;

                DateTime banTime = DateTime.Now.AddSeconds(300);
                TimeSpan isBanned = conf.userPropertys(listIndex).banUntil - DateTime.Now;

                // Comprobar si no está baneado pero no tiene intentos
                if ((isBanned.Minutes < 0 && isBanned.Seconds < 0) && conf.userPropertys(listIndex).nrTry <= 0)
                {
                    conf.userPropertys(listIndex).nrTry = 3;    // Poner el número de intentos a 3
                }

                int intentos = conf.userPropertys(listIndex).nrTry; // Asignar el número de intentos para no tener que acceder todo el rato a la variable

                // Comprobar si está baneado
                if ((isBanned.Minutes > 0 || isBanned.Seconds > 0) && intentos <= 0)
                {
                    messageToSend = string.Format("Your are currently banned, please try again in <b>{0}</b> min and <b>{1}</b> sec", isBanned.Minutes, isBanned.Seconds);
                }
                else
                {
                    if (conf.passWord == userPassword)
                    {
                        conf.userPropertys(listIndex).isAuth = true;
                        text.writeWithColor(string.Format("UserID: {0} added to the auth list.", conf.userPropertys(listIndex).iD));
                    }
                    else
                    {
                        intentos = intentos - 1;
                        if (intentos == 0)
                        {
                            // Banear al usuario
                            conf.userPropertys(listIndex).banUntil = banTime;
                            messageToSend = string.Format("You are now banned for <b>{0}</b> min and <b>{1}</b> sec.", (banTime - DateTime.Now).Minutes, (banTime - DateTime.Now).Seconds);
                        }
                        else
                        {
                            var pS = (intentos == 1) ? "try" : "trys";
                            messageToSend = string.Format("Password is incorrect\nYou have {0} more {1}", intentos, pS);
                        }
                        // Actualizar los intentos del usuario
                        conf.userPropertys(listIndex).nrTry = intentos;
                    }
                }
            }
            #endregion
            //sendWithKeyboard(e.Message.Text, e, bot);
        }
        else
        {
            if (!conf.userPropertys(listIndex).isAuth)
            {
                messageToSend = string.Format("How to use this bot:\n/login YourPassword\n\nYou currently have <b>{0}</b> trys", conf.userPropertys(listIndex).nrTry);
            }
            else
            {
                messageToSend = @"Bot Functions:
/torrent   -   Menu Torrent
/torrentDownload <link>   -   Descarga el torrent indicado
/torrentInfo    -   Información sobre el estado de los torrent
";
            }
        }

        // Send message
        var sentMsg = new Message();
        if (conf.userPropertys(listIndex).isAuth)
        {
            messageToSend = "Authorized Menu";
            var keyboard = new InlineKeyboardMarkup(new[] {
                new[] {
                    new InlineKeyboardButton("User Status", "/status"),
                }
            });
            // Aquí envía el primer teclado inline.            sentMsg = await bot.SendTextMessageAsync(e.Message.Chat.Id, messageToSend, replyMarkup: keyboard, parseMode: ParseMode.Html);
        }
        else
        {
            // Si no está autorizado todos va por mensajes normales.
            sentMsg = await bot.SendTextMessageAsync(e.Message.Chat.Id, messageToSend, replyMarkup: new ReplyKeyboardHide(), parseMode: ParseMode.Html);
        }

        conf.userPropertys(listIndex).lastMsgID = sentMsg.MessageId;    // Set last ID
        // Liberar memoria
        messageToSend = null;
        bot = null;
	}

    /*
	private static async void sendWithKeyboard(string menu, MessageEventArgs e, TelegramBotClient bot)
	{
        var reply = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
		reply.OneTimeKeyboard = true;
		reply.ResizeKeyboard = true;
		reply.Selective = true;

		long chatID = e.Message.Chat.Id;
        int listIndex = conf.getIndexAuthList (e.Message.From.Id);
        int intentos = conf.userPropertys(listIndex).nrTry; // Variable para saber los intentos de introducir contraseña
        bool isAuth = conf.userPropertys(listIndex).isAuth;
        double sec = (double)300; // Tiempo de baneo
        DateTime banUntil = conf.userPropertys(listIndex).banUntil;
        TimeSpan baneado = banUntil - DateTime.Now;
        Message sentMsg = e.Message; // Asignarlo a algo primero

        switch (menu) 
        {
			case "/start":
                if (!isAuth) 
                {
        			reply.Keyboard = new[] 
                    {
        				new[] 
                        {
        					new KeyboardButton ("/auth")
        				}
                    };
                    sentMsg = await bot.SendTextMessageAsync(chatID,"<i>Activando sistemas</i> <b>!</b>", replyMarkup: reply, parseMode: ParseMode.Html);
                    conf.userPropertys(listIndex).lastMsgID = sentMsg.MessageId;
                } 
                else 
                {
                    sendWithKeyboard ("/authMenu", e, bot); // Si está autenticado redirigir al menu de autenticados
                }
            break;

            case "/server":
                if (!isAuth) 
                {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                } 
                else 
                {
                    reply.Keyboard = new[] 
                    {
                        new[] 
                        {
                            new KeyboardButton ("/rebootServer")
                        },
                        new [] 
                        {
                            new KeyboardButton("/unAuth")    
                        },
                        new [] 
                        {
                            new KeyboardButton ("/authMenu")
                        }
                    };
                    sentMsg = await bot.SendTextMessageAsync (chatID, "<i>Sección</i> <b>Server</b>", replyMarkup: reply, parseMode: ParseMode.Html);

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
                    sentMsg = await bot.SendTextMessageAsync(chatID, "El <b>sistema</b> se va a reiniciar en <i>" + rebootTime + "</i> segundos", parseMode: ParseMode.Html);
                }

            break;

			case "/torrent":
                if (!isAuth) 
                {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                } 
                else 
                {
					reply.Keyboard = new[] {
						new[] 
                        {
							new KeyboardButton ("/uploadTorrent")
						},
						new[] 
                        {
                            new KeyboardButton ("/listTorrent"),
							new KeyboardButton ("/deleteTorrent")
						},
						new [] 
                        {
							new KeyboardButton ("/authMenu")
						}
					};
                    sentMsg = await bot.SendTextMessageAsync(chatID, "<i>Sección</i> <b>Torrent</b>", replyMarkup: reply, parseMode: ParseMode.Html);
				}
			break;

            case "/auth":
                if (!isAuth && intentos >= 0 && baneado.Seconds <= 0)
                {
                    conf.userPropertys(listIndex).inputText = true;
                    sentMsg = await bot.SendTextMessageAsync(chatID, "<i>Tienes</i> <b>" + intentos + "</b> <i>intentos disponibles.</i>", parseMode: ParseMode.Html);
                    sentMsg = await bot.SendTextMessageAsync(chatID, "<i>Porfavor introduce tu contraseña</i>", parseMode: ParseMode.Html);
                }
                else if(intentos == 0 || baneado.Minutes > 0 || baneado.Seconds > 0)
                {
                    sendWithKeyboard("authFail", e, bot);
                }
                else
                {
                    sendWithKeyboard("/authMenu", e, bot);
                }
            break;

            case "/unAuth":
                if (!isAuth) 
                {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                } 
                else 
                {
                    conf.userPropertys(listIndex).isAuth = false;
                    // Debug for console
                    text.writeWithColor ("UserID: ", ConsoleColor.DarkYellow);
                    text.writeWithColor (e.Message.From.Id.ToString(), ConsoleColor.DarkRed);
                    text.writeWithColor (" removed from the auth list. ", ConsoleColor.DarkYellow, true);
                    sentMsg = await bot.SendTextMessageAsync (chatID, "<i>Ya</i> <b>no</b> <i>tienes autorización en este bot</i>.", parseMode: ParseMode.Html);
                };
                
            break;

			case "/authMenu":
                if (isAuth) 
                {
                    reply.Keyboard = new [] 
                    {
                            new[] 
                            {
                                new KeyboardButton ("/server")
                            },
                            new[] 
                            {
                                new KeyboardButton ("/torrent"),
                            }
                        };
                    sentMsg = await bot.SendTextMessageAsync (chatID, "<i>Menú con privilegios</i>", replyMarkup: reply, parseMode: ParseMode.Html);
                } 
                else 
                {
                    sendWithKeyboard ("noAuth", e, bot); // Ir a la sección de no autorización
                }
			break;

            case "authOk":
                reply.Keyboard = new[] 
                {
                    new[] 
                    {
                        new KeyboardButton ("/server")
                    },
                    new[] 
                    {
                        new KeyboardButton ("/torrent"),
                    }
                };
                conf.userPropertys(listIndex).inputText = false;
                conf.userPropertys(listIndex).isAuth = true;
                sentMsg = await bot.SendTextMessageAsync(chatID, "<i>Estás logueado</i>", replyMarkup: reply, parseMode: ParseMode.Html);
			break;
				
            case "authFail":
                if (intentos == 0 && baneado.Minutes <= 0 && baneado.Seconds <= 0)
                {
                    conf.userPropertys(listIndex).nrTry = (int)3;
                    intentos = 3;
                }

                intentos = intentos - 1;

                if (intentos >= 0 && baneado.Minutes <= 0 && baneado.Seconds <= 0)
                {
                    if (intentos == 0)
                    {
                        conf.userPropertys(listIndex).banUntil = DateTime.Now.AddSeconds(sec); // Añade baneo durante sec segundos
                    }
                    conf.userPropertys(listIndex).nrTry = intentos;
                    sentMsg = await bot.SendTextMessageAsync(chatID, "<i>Contraseña incorrecta NO.OB</i>", parseMode: ParseMode.Html);
                    sendWithKeyboard("/auth", e, bot);
                }
                else if (intentos < 0 && baneado.Minutes > 0 && baneado.Seconds > 0)
                {
                    sentMsg = await bot.SendTextMessageAsync(chatID, "<i>No te quedan intentos</i>", parseMode: ParseMode.Html);
                    sentMsg = await bot.SendTextMessageAsync(chatID, "<i>Prueba de nuevo en</i> <b>" + baneado.Minutes + "</b> <i>minuto/s y</i> <b>" + baneado.Seconds + "</b> <i>segundo/s</i>", parseMode: ParseMode.Html);
                }
            break;

            case "noAuth":
                reply.Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("/auth"),
                    }
                };
                sentMsg = await bot.SendTextMessageAsync (chatID, "<i>No tienes</i> <b>acceso</b> <i>a este comando</i>", replyMarkup: reply, parseMode: ParseMode.Html);
            break;

            default:
                reply.Keyboard = new [] 
                {
                    new[] 
                    {
                        new KeyboardButton ("/start")
                    }
                };
                sentMsg = await bot.SendTextMessageAsync (chatID, "Comando <b>no</b> reconocido", replyMarkup: reply, parseMode: ParseMode.Html);
            break;
		}
        conf.userPropertys(listIndex).lastMsgID = sentMsg.MessageId;
	}*/
}