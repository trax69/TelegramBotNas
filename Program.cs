using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net; // Variable para descargar de internet

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
    static InlineKeyboardMarkup keyboard; //Variable para el teclado estatico

	public static void Main(string[] args)
	{
        string filePath = AppDomain.CurrentDomain.BaseDirectory;
        string fileName = "config.json";

        /* Iniciando Variables */
        conf = new botSetting (filePath + fileName);
        filePath = fileName = null;
        text = new consoleTweaks();

        // Dibujamos el teclado estatico para tenerlo siempre accesible desde el inicio y aprovecharlo
        keyboard = new InlineKeyboardMarkup(new[] 
        {
                new[]
                    {
                    new InlineKeyboardButton("\ud83d\udda5️ Servidor"),
                    new InlineKeyboardButton("⬇️️ Torrents"),

                    },
                    new[]
                    {
                    new InlineKeyboardButton("\ud83d\udeaa Salir"),
                    }
        });
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
                FirstName = "B.C.",
                LastName = "M.",
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
        // PARA COJER EL ID DEL ULTIMO MENSAJE A ESE USUARIO USARIO CON conf.userPropertys(listIndex).lastMsgID mejor con e.CallbackQuery.Message.MessageId;
        var bot = (TelegramBotClient)sender;
		string msgToSend = "";
        string texto = "";
		msgToSend = (e.CallbackQuery.Data);
        //  http://emojipedia.org/ para insertar emojis en los textos

        var menu = new InlineKeyboardMarkup();
        if (msgToSend == "\ud83d\udd19 Volver")
        {
            texto = "-----------------------Menú----------------------";
        }
        else if (msgToSend == "⬇️️ Torrents")
        {
            ///////////////////////// MENU TORRENTS /////////////////////////////////////////////
            // Menu para los torrents cambiamos el teclado modificando el anterior //////////////
            /////////////////////////////////////////////////////////////////////////////////////
            texto = "ℹ️️ En este apartado es donde puedes desde mandarme los torrents que quieres que sean descargados. \n\nℹ️️ Listar los torrents que están siendo descargados. \n\nℹ️️ Incluso Borrar los torrents que estaban siendo descargados.";
            menu = new InlineKeyboardMarkup(new[] {
                new[]
                    {
                    new InlineKeyboardButton("⬆️️ Upload torrent"),
                    new InlineKeyboardButton("\ud83d\udcc3 List Torrent"),

                    },
                    new[]
                    {
                    new InlineKeyboardButton("\ud83d\uddd1️ Delete torrent"),
                    },
                    new[]
                    {
                    new InlineKeyboardButton("\ud83d\udd19 Volver"),

                    }
            });

        }
        else if (msgToSend == "\ud83d\udda5️ Servidor")
        {
             ///////////////////////// MENU SERVIDOR /////////////////////////////////////////////
            //////////////////////// Menu para las opciones del servidor  ///////////////////////
            /////////////////////////////////////////////////////////////////////////////////////
            texto = "️ℹ️️ En este apartado usted podrá manejar su servidor NAS del mismo modo que si lo hiciese desde casa";
            menu = new InlineKeyboardMarkup(new[] {
                new[]
                    {
                    new InlineKeyboardButton("\ud83d\udd04 Reboot"),
                    new InlineKeyboardButton("\ud83d\udcf4 Shutdown"),

                    },
                    new[]
                    {
                    new InlineKeyboardButton("\ud83d\udd19 Volver"),
                    }
            });

        }
        else if (msgToSend == "\ud83d\udeaa Salir")
        {
            // Solo cambiamos el texto por que lo quiere cerrar
            texto = "ℹ️️ ️Recuerda que siempre puedes iniciar el menú con /start . \n\n\ud83d\udc4b Adios y hasta pronto =) \n\n";
        }
        else if (msgToSend == "\ud83d\udcf4 Shutdown" || msgToSend == "\ud83d\udd04 Reboot" )
        {
            ///////////////////////// OPCIONES SERVIDOR  /////////////////////////////////////////////
            ///////////////////////          PARA REINICIAR Y APAGAR        ///////////////////////
            /////////////////////////////////////////////////////////////////////////////////////
            menu = new InlineKeyboardMarkup(new[]
           {

                new[]
                    {
                    new InlineKeyboardButton("✅Si"),
                    new InlineKeyboardButton("❌No"),
                    },
            });
            if (msgToSend == "\ud83d\udcf4 Shutdown")
            {
                texto = "⚠️ El servidor se apagará <b>\nen 10 segundos.</b> \n\n<b>¿Desea continuar?</b>";
            }
            else
            {
                texto = "⚠️ El servidor se reiniciará <b>\nen 10 segundos.</b> \n\n<b>¿Desea continuar?</b>";
            }
          }
        else if (msgToSend == "✅Si" && e.CallbackQuery.Message.Text.Contains("apagará"))
        {
            // Tiempo de reinicio en segundos
            string rebootTime = "10";
            if (botChecks.isLinux())
            {
                Process.Start("shutdown", "-h -t " + rebootTime);
            }
            else
            {
                Process.Start("shutdown.exe", "-s -t " + rebootTime);
            }

            texto = "-----------------------Menú----------------------";
            msgToSend = "\ud83d\udd19 Volver";
        }
        else if (msgToSend == "✅Si" && e.CallbackQuery.Message.Text.Contains("reiniciará"))
        {
            string rebootTime = "10";
            if (botChecks.isLinux())
            {
                Process.Start("shutdown", "-r -t " + rebootTime);
            }
            else
            {
                Process.Start("shutdown.exe", "-r -t " + rebootTime);
            }

            texto = "-----------------------Menú----------------------";
            msgToSend = "\ud83d\udd19 Volver";
        }

        else if (msgToSend == "❌No")
        {
            texto = "-----------------------Menú----------------------";
            msgToSend = "\ud83d\udd19 Volver";
        }


        // Revisar posibles errores a partir de aqui 
        if (e.CallbackQuery.Message.Text != texto && msgToSend != "\ud83d\udd19 Volver" && msgToSend != "\ud83d\udeaa Salir")
        {
            // Manda el teclado variable generado mas arriba
            await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, texto, ParseMode.Html, false, replyMarkup: menu);
        }
        else if (msgToSend == "\ud83d\udd19 Volver")
        {
            // Para mandar el teclado fijo en lugar del variable
            await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, texto, ParseMode.Html, false, replyMarkup: keyboard);
        }
        else if (msgToSend == "\ud83d\udeaa Salir")
        { 
            // Este es para ocultar el teclado al pulsar salir
            await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id,e.CallbackQuery.Message.MessageId,texto,ParseMode.Html,false);
        }
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
                            e.Message.From.Username, nrTry: (int)3);
        }

        listIndex = conf.getIndexAuthList(e.Message.From.Id);   // Poner el listIndex bien después de añadir a la lista

        string messageToSend = "⚠️ Comando no reconocido ⚠️";
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
                    messageToSend = "ℹ️️ Logueate con el comando.\n/login tucontraseña";
                }
            }
            #region userLogin
            else if (e.Message.Text.StartsWith("/login", StringComparison.OrdinalIgnoreCase))
            {
                if (conf.userPropertys(listIndex).isAuth) return;   // Si el usuario ya está autorizado pasar en moto de que ha puesto login ... pobre tonto.
                string userPassword = null;
                if (e.Message.Text.StartsWith("/login ", StringComparison.OrdinalIgnoreCase))
                {
                    string[] pwContainer = e.Message.Text.Split(char.Parse(" "));
                    userPassword = pwContainer[1];
                    pwContainer = null;
                }
                else
                {
                    messageToSend = "⚠️ Debes escribir \n/login tucontraseña";
                }
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
                    messageToSend = string.Format("❌ Estas baneado, vuelva a intentarlo en <b>{0}</b> minutos y <b>{1}</b> segundos.", isBanned.Minutes, isBanned.Seconds);
                }
                else
                {
                    if (conf.passWord == userPassword)
                    {
                        conf.userPropertys(listIndex).isAuth = true;
                        text.writeWithColor(string.Format("UserID: {0} added to the auth list.", conf.userPropertys(listIndex).iD));
                    }
                    else if (userPassword != null)
                    {
                        intentos = intentos - 1;
                        if (intentos == 0)
                        {
                            // Banear al usuario
                            conf.userPropertys(listIndex).banUntil = banTime;
                            messageToSend = string.Format("Has sido baneado <b>{0}</b> minutos y <b>{1}</b> segundos.", (banTime - DateTime.Now).Minutes, (banTime - DateTime.Now).Seconds);
                        }
                        else
                        {
                            var pS = (intentos == 1) ? "intento" : "intentos";
                            messageToSend = string.Format("❌ Contraseña incorrecta\nTienes {0} {1} mas", intentos, pS);
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
                // Falta controlar que si no se trata de una url aunque tenga .torrent se lo pase por los huevos
                if (e.Message.Text.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
                {
                    WebClient webClient = new WebClient();
                    string nombre = "";
                    string[] archivo = (e.Message.Text.Split(char.Parse("/")));
                    foreach (string i in archivo)
                    {
                        if (i.Contains(".torrent"))
                            nombre = i; 
                    }
                    // aqui habria que preguntar la ruta donde se quiere guardar para añadirlo a descargas (por defecto en el path del bot)
                   webClient.DownloadFile(e.Message.Text,nombre);
                   await bot.SendTextMessageAsync(e.Message.Chat.Id, "Torrent " + nombre + " añadido", parseMode: ParseMode.Html);
                }

                /*messageToSend = @"Bot Functions:
                /torrent   -   Menu Torrent
                /torrentDownload <link>   -   Descarga el torrent indicado
                /torrentInfo    -   Información sobre el estado de los torrent 
";*/
            }
        }

        // Send message
        var sentMsg = new Message();
        if (conf.userPropertys(listIndex).isAuth)
        {
            messageToSend = "-----------------------Menú----------------------";
            // Aquí envía el primer teclado inline.
            if (e.Message.Text.Contains("/start") || conf.userPropertys(listIndex).isAuth)
            {
                sentMsg = await bot.SendTextMessageAsync(e.Message.Chat.Id, messageToSend, replyMarkup: keyboard, parseMode: ParseMode.Html);
            }
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
}