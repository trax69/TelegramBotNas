using System;
using System.IO;
// using System.Collections.Generic;
// using System.Configuration;
using System.Diagnostics;
// using System.Management;
// using System.Threading.Tasks;
using System.Net; // Para descargar de internet
using Telegram.Bot;
using Telegram.Bot.Args;
// using Telegram.Bot.Exceptions;
// using Telegram.Bot.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using botSettings;
using botSettings.Checks;
// using botSettings.Enums;

class nasBot
{
    static string botKey;
    static string passWd;
    static string pathtorrent; //path de donde deben guardarse los torrents
    static string pathincomplete;
    static string pathcompleted;
    static string messageToSend; //Variable global para mandar mensaje
    static botSetting conf;
    static consoleTweaks text; // Variable para gestionar la consola
    static InlineKeyboardMarkup keyboard; //Variable para el teclado estatico
    static InlineKeyboardMarkup menu;
    static TelegramBotClient bot;
    public static void Main(string[] args)
    {
        string filePath = AppDomain.CurrentDomain.BaseDirectory;
        string fileName = "config.json";
        /* Iniciando Variables */
        conf = new botSetting(filePath + fileName);
        filePath = fileName = null;
        text = new consoleTweaks();

        // Dibujamos el teclado estatico para tenerlo siempre accesible desde el inicio y aprovecharlo
        keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                new InlineKeyboardButton("\ud83d\udda5️ Servidor", "/server"),
                new InlineKeyboardButton("⬇️️ Torrents", "/torrent")
            },
            new[]
            {
                new InlineKeyboardButton ("\ud83d\udc64 Cuenta", "/account")
            },
            new[]
            {
                new InlineKeyboardButton("\ud83d\udeaa Salir", "/salir")
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
                conf.loadConfig();
                // Separa el string en un array dividiendo por el '='
                string[] data = arg.Split((char)'=');
                // Si la primera parte del array contiene 'key', asignamos en los ajustes
                if (data[0].Contains("key"))
                {
                    // Save Token
                    conf.botToken = data[1];
                }
                else if (data[0].Contains("pW"))
                {
                    // Save passWord
                    conf.passWord = data[1];
                }
                text.writeWithColor("");
                conf.saveConfig(); // Guardar la config
                Environment.Exit(0);
            }
        }

        /* Titulo de la APP ! :-) */
        text.Title("Welcome to Telegram Bot App ! " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), char.Parse("#"), ConsoleColor.Blue);

        /* Cargar configuración del bot */
        conf.loadConfig();

        /* Asignar Variables */
        botKey = conf.botToken;
        passWd = conf.passWord;
        pathtorrent = conf.torrentFilesPath;
        pathincomplete = conf.incompleteFilesPath;
        pathcompleted = conf.completedFilesPath;


        if (string.IsNullOrEmpty(botKey) || string.IsNullOrWhiteSpace(botKey))
        {
            botChecks chk = new botChecks();
            while (chk.checkToken(botKey) == false)
            {
                botKey = createToken();
            }
            conf.botToken = botKey;
            chk = null;
            Console.WriteLine();
        }

        if (string.IsNullOrEmpty(passWd) || string.IsNullOrWhiteSpace(passWd))
        {
            botChecks chk = new botChecks();
            while (chk.checkPW(passWd) == false)
            {
                passWd = createPassword();
            }
            conf.passWord = passWd;
            chk = null;
            Console.WriteLine();
        }

        if (string.IsNullOrEmpty(pathtorrent) || string.IsNullOrWhiteSpace(pathtorrent))
        {
            botChecks chk = new botChecks();
            while (chk.checkPath(pathtorrent) == false)
            {
                pathtorrent = createTorrent("torrent");
            }
            // Comprobar que la ruta proporcionada exista
            conf.torrentFilesPath = pathtorrent;
            chk = null;
            Console.WriteLine();
        }

        if (string.IsNullOrEmpty(pathincomplete) || string.IsNullOrWhiteSpace(pathincomplete))
        {
            botChecks chk = new botChecks();
            while (chk.checkPath(pathincomplete) == false)
            {
                pathincomplete = createTorrent("incomplete");
            }
            // Comprobar que la ruta proporcionada exista
            conf.incompleteFilesPath = pathincomplete;
            chk = null;
            Console.WriteLine();
        }

        if (string.IsNullOrEmpty(pathcompleted) || string.IsNullOrWhiteSpace(pathcompleted))
        {
            botChecks chk = new botChecks();
            while (chk.checkPath(pathcompleted) == false)
            {
                pathcompleted = createTorrent("completed");
            }
            // Comprobar que la ruta proporcionada exista
            conf.completedFilesPath = pathcompleted;
            chk = null;
            Console.WriteLine();
        }

        conf.saveConfig();
        text.writeWithColor("Using Token: ");
        text.writeWithColor(conf.botToken, ConsoleColor.Magenta, true);
        text.writeWithColor("Using Password: ");
        text.writeWithColor(conf.passWord, ConsoleColor.Magenta, true);
        text.writeWithColor("Using path torrent: ");
        text.writeWithColor(conf.torrentFilesPath, ConsoleColor.Magenta, true);
        text.writeWithColor("Using path incomplete: ");
        text.writeWithColor(conf.incompleteFilesPath, ConsoleColor.Magenta, true);
        text.writeWithColor("Using path completed: ");
        text.writeWithColor(conf.completedFilesPath, ConsoleColor.Magenta, true);
        Console.WriteLine();

        text.writeWithColor("Starting bot... ");
        // Asignar el valor del token al bot y asignar el bot a una variable
        var bot = new TelegramBotClient(botKey);
        // Ponerle nombre a la consola
        Console.Title = bot.GetMeAsync().Result.Username + " - Listening";

        /* Iniciar Bot*/
        bot.StartReceiving();

        //MONITORIZARE CARPETAS
        FileSystemWatcher watcher = new FileSystemWatcher(); // Para monitorizar carpetas
        watcher = new FileSystemWatcher();
		watcher.Path = pathcompleted + Path.DirectorySeparatorChar;
        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        watcher.Filter = "*.*";
        // watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnChanged);
        watcher.EnableRaisingEvents = true;
        // MONITORIZAR CARPETAS

        /* Hacer que cuando se disparen los eventos llamen a un método */
        bot.OnMessage += BotOnMessageReceived;
        bot.OnInlineQuery += BotOnInlineQueryReceived;
        bot.OnInlineResultChosen += BotOnInlineResultChosen;
        bot.OnCallbackQuery += BotOnCallbackQueryReceived;
        

        text.writeWithColor("Ok. ", ConsoleColor.DarkGreen, true);

        /* Especifico para Windows para que no se cierre automaticamente la ventana */
        Console.BackgroundColor = ConsoleColor.White;
        text.writeWithColor("Write QUIT to exit execution\n", ConsoleColor.Black, true);

        // Qué la consola no se cierre
        string consoleMenu;
        // Bucle comandos de la consola

        while ((consoleMenu = Console.ReadLine().ToLower()) != "quit")
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
        conf.saveConfig();
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
        Console.WriteLine();
        text.writeWithColor("Unable to find usable token, please write it now: ", newLine: true);
        rToken = Console.ReadLine();
        text.writeWithColor("Is the Token correct ? (Y/N): ");
        if (!(Console.ReadKey().Key == ConsoleKey.Y))
        {
            createToken();
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
        Console.WriteLine();
        text.writeWithColor("Unable to find usable password, please write it now: ", newLine: true);
        rPW = Console.ReadLine();
        text.writeWithColor("Is the password correct ? (Y/N): ");
        if (!(Console.ReadKey().Key == ConsoleKey.Y))
        {
            createPassword();
        }
        return rPW;
    }

    /// <summary>
    /// Creates the path torrent.
    /// </summary>
    /// <returns>torrent path.</returns>
    public static string createTorrent(string prueba)
    {
        string torrent = "";
        Console.WriteLine();
        text.writeWithColor("Unable to find usable path to " + prueba + ", please write it now: ", newLine: true);
        torrent = Console.ReadLine();
        text.writeWithColor("Is the correct path ? (Y/N): ");
        if (!(Console.ReadKey().Key == ConsoleKey.Y))
        {
            createTorrent(torrent);
        }
        return torrent;
    }

    static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
    {
        var bot = (TelegramBotClient)sender;
        text.writeWithColor("InlineQuery (ID: ");
        text.writeWithColor(e.InlineQuery.Id, ConsoleColor.Blue);
        text.writeWithColor(" FromID: ");
        text.writeWithColor(e.InlineQuery.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor("): ");
        text.writeWithColor(e.InlineQuery.Query, ConsoleColor.DarkBlue, true);

        InlineQueryResult[] results =
        {
            new InlineQueryResultContact
            {
                Id = "1",
                FirstName = "B.C.M.",
                PhoneNumber = "+34 666777888",
                InputMessageContent = new InputContactMessageContent
                {
                    FirstName = "Big Cash Monkeys",
                    PhoneNumber = "+34 666777888"
                }
            }
        };

        await bot.AnswerInlineQueryAsync(e.InlineQuery.Id, results, 0, true);
    }

    static void BotOnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
    {
        text.writeWithColor("InlineResult (InlineMsgID: ");
        text.writeWithColor(e.ChosenInlineResult.InlineMessageId, ConsoleColor.Blue);
        text.writeWithColor(" ResultID: ");
        text.writeWithColor(e.ChosenInlineResult.ResultId, ConsoleColor.Blue);
        text.writeWithColor(" FromID: ");
        text.writeWithColor(e.ChosenInlineResult.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor(" ForQuery: ");
        text.writeWithColor(e.ChosenInlineResult.Query, ConsoleColor.Blue);
        text.writeWithColor(" )", newLine: true);
    }

    static void OnChanged(object source, FileSystemEventArgs e)
    {
        int prueba = conf.userPropertys(0).iD;
        //AL CAMBIAR ALGO EN LA CARPETA COMPLETADOS MANDAR UN MENSAJE DE QUE EL TORRENT HA SIDO DESCARGADO
        bot.SendTextMessageAsync(prueba, "✅ El torrent " + e.Name + " ha sido totalmente descargado.", parseMode: ParseMode.Html);
    }
    // Para obtener datos del PC en windows
    // https://msdn.microsoft.com/en-us/library/aa389273.aspx para sacar los datos de windows
    private static string GetComponent(string hwclass, string syntax)
    {
        // MEtodo para obtener datos del sistema
        string status = "";
        foreach (var item in new System.Management.ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + hwclass).Get())
        {
            status = Convert.ToString(item[syntax].ToString());
        }
        return status;
    }

    private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
    {
        bot = (TelegramBotClient)sender;
        string msgToSend = e.CallbackQuery.Data;
        string texto = "";
        string rebootTime = "10";
        // http://emojipedia.org/ para insertar emojis en los textos

        if (msgToSend == "/volver")
        {
            texto = "-----------------------Menú----------------------";
        }
        else if (msgToSend == "/torrent")
        {
            ///////////////////////// MENU TORRENTS /////////////////////////////////////////////
            // Menu para los torrents cambiamos el teclado modificando el anterior //////////////
            /////////////////////////////////////////////////////////////////////////////////////
            texto = "ℹ️️ En este apartado es donde puedes desde mandarme los torrents que quieres que sean descargados. \n\nℹ️️ Listar los torrents que están siendo descargados. \n\nℹ️️ Incluso Borrar los torrents que estaban siendo descargados.";
            menu = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton("⬆️️ Añadir torrent", "/uploadTorrent"),
                    new InlineKeyboardButton("\ud83d\udcc3 Listar Torrent", "/listTorrent")
                },
                new[]
                {
                    new InlineKeyboardButton("\ud83d\uddd1️ Eliminar torrent", "/deleteTorrent")
                },
                new[]
                {
                    new InlineKeyboardButton("\ud83d\udd19 Volver", "/volver")
                }
            });

        }
        else if (msgToSend == "/uploadTorrent")
        {
            texto = "ℹ️️ Envia un texto unicamente con el enlace. \n\n❕Ejemplo: <i>www.torrent.com/Pelicula.torrent</i>";
        }
        else if (msgToSend == "/listTorrent")
        {
            int num = 0;
            texto = "🗒️Listado torrents descargando🗒️ \n\n";
            DirectoryInfo Directorio = new DirectoryInfo(pathincomplete);
            foreach (var fi in Directorio.GetFiles("*"))
            {
                num = num + 1;
				texto = texto + "<b>" +num + ".</b> " + fi.Name.Substring(0,20) + "[...] " + fi.Length + "Gb" + "\n";
            }

            foreach (var fi in Directorio.GetDirectories("*"))
            {
                num = num + 1;
                texto = texto + "<b>" + num + ".</b> " + fi.Name + "\n";
            }

        }
	    else if (msgToSend == "/server")
	    {
            ///////////////////////// MENU SERVIDOR /////////////////////////////////////////////
            //////////////////////// Menu para las opciones del servidor ///////////////////////
            /////////////////////////////////////////////////////////////////////////////////////
            texto = "️ℹ️️ En este apartado usted podrá manejar su servidor NAS del mismo modo que si lo hiciese desde casa";
            menu = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton("\ud83d\udd04 Reiniciar", "/serverReboot"),
                    new InlineKeyboardButton("\ud83d\udcf4 Apagar", "/serverShutdown"),

                },
                new[]
                {
                    new InlineKeyboardButton("🔎 Estado", "/status"),
                    new InlineKeyboardButton("🤖 Detener bot", "/stopbot"),
                },
                new[]
                {
                    new InlineKeyboardButton("\ud83d\udd19 Volver", "/volver"),
                }
            });
        }
	    else if (msgToSend == "/status")
	    {
            if (!botChecks.isLinux())
            {
                // Para sacar la informacion del sistema para mostrar
                texto =
                (
                "⚙️S.O.: " + (GetComponent("Win32_OperatingSystem", "Name")).Split('|')[0] +
                "\n🖥️CPU: " + GetComponent("Win32_Processor", "Name") +
                "\n🌐Nº Nucleos: " + GetComponent("Win32_Processor", "NumberOfCores") +
                "\n👨‍💻Arquitectura: " + GetComponent("Win32_OperatingSystem", "OSArchitecture") +
                "\n📡NetBios: " + GetComponent("Win32_NetworkAdapter", "SystemName") +
                "\n💾RAM Libre: " + GetComponent("Win32_OperatingSystem", "FreePhysicalMemory") + // EN MB faltaria dividir 1024
                "\n💾RAM Total: " + GetComponent("Win32_OperatingSystem", "TotalVisibleMemorySize") +
                "\n🕰️Time up: " + GetComponent("Win32_OperatingSystem", "LastBootUpTime") // Revisar esta en segundos notacion cientifica ¿?
                );
            }
            else
            {
                texto = "Estamos trabajando en ello";
            }
        }
	    else if (msgToSend == "/salir")
	    {
            // Solo cambiamos el texto por que lo quiere cerrar
            texto = "ℹ️️ ️Recuerda que siempre puedes iniciar el menú con /start . \n\n\ud83d\udc4b Adios y hasta pronto =) \n\n";
        }
	    else if (msgToSend == "/serverReboot" || msgToSend == "/serverShutdown" || msgToSend == "/stopbot")
	    {
            ///////////////////////// OPCIONES SERVIDOR /////////////////////////
            /////////////////////// PARA REINICIAR Y APAGAR /////////////////////
            /////////////////////////////////////////////////////////////////////////////////////
            menu = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton("✅Si","/si"),
                    new InlineKeyboardButton("❌No","/no"),
                },
            });

            //Controlamos si lo que quieres es apagar o reiniciar y cambiamos el texto del teclado
            if (msgToSend == "/serverShutdown")
            {
                texto = "⚠️ El servidor se apagará <b>\nen 10 segundos.</b> \n\n<b>¿Desea continuar?</b>";
            }
            else if (msgToSend == "/serverReboot")
            {
                texto = "⚠️ El servidor se reiniciará <b>\nen 10 segundos.</b> \n\n<b>¿Desea continuar?</b>";
            }
            else if (msgToSend == "/stopbot")
            {
                texto = "⚠️¿Estas seguro de querer parar el bot?⚠️\n⚠️Si lo haces tendra que ser arrancado manualmente⚠️";
            }
        }
	    else if (msgToSend == "/si" && e.CallbackQuery.Message.Text.Contains("apagará"))
	    {
            if (botChecks.isLinux())
            {
                Process.Start("shutdown", "-h -t " + rebootTime);
            }
            else
            {
                Process.Start("shutdown.exe", "-s -t " + rebootTime);
            }

            texto = "-----------------------Menú----------------------";
            msgToSend = "/salir";
            texto = "ℹ️️ ️Recuerda que siempre puedes iniciar el menú con /start . \n\n\ud83d\udc4b Adios y hasta pronto =) \n\n";
        }
	    else if (msgToSend == "/si" && e.CallbackQuery.Message.Text.Contains("reiniciará"))
	    {
            if (botChecks.isLinux())
            {
                Process.Start("shutdown", "-r -t " + rebootTime);
            }
            else
            {
                Process.Start("shutdown.exe", "-r -t " + rebootTime);
            }

            texto = "-----------------------Menú----------------------";
            msgToSend = "/volver";
        }
	    else if (msgToSend == "/si" && e.CallbackQuery.Message.Text.Contains("parar el bot"))
	    {
            msgToSend = "/salir";
            texto = "ℹ️️ ️Recuerda que siempre puedes iniciar el menú con /start . \n\n\ud83d\udc4b Adios y hasta pronto =) \n\n";
        }
	    else if (msgToSend == "/no")
	    {
            texto = "-----------------------Menú----------------------";
            msgToSend = "/volver";
        }

        ////////////////////////////////////////////////////////////////////////////
        /// Aqui termina el apartado del servidor para reiniciar o apagar ///
        ////////////////////////////////////////////////////////////////////////////


        // Revisar posibles errores a partir de aqui
	    if (texto == "")
	    {
            texto = "‼️ Opción <b>" + msgToSend + " </b>aún no habilitada ‼️";
            if (e.CallbackQuery.Message.Text != texto)
            {
		        await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, texto, ParseMode.Html, false, replyMarkup: keyboard);
            }
        }
	    else if (e.CallbackQuery.Message.Text != texto && msgToSend != "/volver" && msgToSend != "/salir")
	    {
            // Manda el teclado variable generado mas arriba
            await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, texto, ParseMode.Html, false, replyMarkup: menu);
        }
	    else if (msgToSend == "/volver")
	    {
            // Para mandar el teclado fijo en lugar del variable
            await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, texto, ParseMode.Html, false, replyMarkup: keyboard);
        }
	    else if (msgToSend == "/salir")
	    {
            // Este es para ocultar el teclado al pulsar salir
		    await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, texto, ParseMode.Html, false);
            if(e.CallbackQuery.Message.Text.Contains("parar el bot"))
		    {
                bot.StopReceiving();
                Environment.Exit(0);
            } 

        }
    }
    /// <summary>
    /// On message Recieved Method
    /// </summary>
    /// <param name="sender">TelegramBotClient as sender (Object)</param>
    /// <param name="e">MessageEventArgs as e</param>
	static async void BotOnMessageReceived(object sender, MessageEventArgs e)
    	{
        // Si no es un mensaje de texto pasar en moto de el qué sino da error !
        if (e.Message.Type != MessageType.TextMessage) return;
        // Crea un nuevo objeto TelegramBotClient apartir del sender para poder hacer cosas con los mensajes
        var bot = (TelegramBotClient)sender;
        // Bloque debug para la consola
        text.writeWithColor("Message (ID:");
        text.writeWithColor(e.Message.MessageId.ToString(), ConsoleColor.Blue);
        text.writeWithColor(" ChatID:");
        text.writeWithColor(e.Message.Chat.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor(" FromID:");
        text.writeWithColor(e.Message.From.Id.ToString(), ConsoleColor.Blue);
        text.writeWithColor("): ");
        text.writeWithColor(e.Message.Text, ConsoleColor.DarkCyan, true);
        int listIndex = conf.getIndexAuthList(e.Message.From.Id);
        if (listIndex == -1)
        {
            conf.addUser(e.Message.From.Id, e.Message.From.FirstName, e.Message.From.LastName,
            e.Message.From.Username, nrTry: (int)3);
        }

        listIndex = conf.getIndexAuthList(e.Message.From.Id); // Poner el listIndex bien después de añadir a la lista

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
                    messageToSend = "ℹ️️ Logueate con el comando.\n/login &lt;contraseña&gt;";
                }
            }
            #region userLogin
            else if (e.Message.Text.StartsWith("/login", StringComparison.OrdinalIgnoreCase))
            {
                if (conf.userPropertys(listIndex).isAuth) return; // Si el usuario ya está autorizado pasar en moto de que ha puesto login ... pobre tonto.
                string userPassword = null;
                if (e.Message.Text.StartsWith("/login ", StringComparison.OrdinalIgnoreCase))
                {
                    string[] pwContainer = e.Message.Text.Split(char.Parse(" "));
                    userPassword = pwContainer[1];
                    pwContainer = null;
                }
                else
                {
                    messageToSend = "⚠️ Debes escribir \n/login &lt;contraseña&gt;";
                }
                DateTime banTime = DateTime.Now.AddSeconds(300);
                TimeSpan isBanned = conf.userPropertys(listIndex).banUntil - DateTime.Now;

                // Comprobar si no está baneado pero no tiene intentos
                if ((isBanned.Minutes < 0 && isBanned.Seconds < 0) && conf.userPropertys(listIndex).nrTry <= 0)
                {
                    conf.userPropertys(listIndex).nrTry = 3; // Poner el número de intentos a 3
                }

                int intentos = conf.userPropertys(listIndex).nrTry; // Asignar el número de intentos para no tener que acceder todo el rato a la variable

                // Comprobar si está baneado
                if ((isBanned.Minutes > 0 || isBanned.Seconds > 0) && intentos <= 0)
                {
                    messageToSend = string.Format("❌ Est&aacute; baneado, vuelva a intentarlo en <b>{0}</b> minutos y <b>{1}</b> segundos.", isBanned.Minutes, isBanned.Seconds);
                }
                else
                {
                    if (conf.passWord == userPassword)
                    {
                        messageToSend = "-----------------------Menú----------------------";
                        conf.userPropertys(listIndex).isAuth = true;
                        text.writeWithColor(string.Format("UserID: {0} added to the auth list.", conf.userPropertys(listIndex).iD));
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, messageToSend, replyMarkup: keyboard, parseMode: ParseMode.Html);
                        conf.saveConfig();
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
            else
            {
                messageToSend = "⚠️ Comando no reconocido ⚠️";
            }
            #endregion

        }
        else
        {
            if (!conf.userPropertys(listIndex).isAuth)
            {
		        messageToSend = string.Format("Como usar este bot:\n/login &lt;contraseña&gt;\n\nActualmente tienes <b>{0}</b> intentos", conf.userPropertys(listIndex).nrTry);
            }
            else
            {
                /////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////// AÑADIR TORRENTS ///////////////////
                /////////////////////////////////////////////////////////////////////////////////////////
                // Falta controlar que si no se trata de una url aunque tenga .torrent se lo pase por los huevos
		        if (e.Message.Text.EndsWith(".torrent", StringComparison.InvariantCultureIgnoreCase))
                {
                    WebClient webClient = new WebClient();
                    string nombre = "";
		            bool descargado;
                    string[] archivo = (e.Message.Text.Split(char.Parse("/")));
                    foreach (string i in archivo)
                    {
                        if (i.EndsWith(".torrent", StringComparison.InvariantCultureIgnoreCase))
                        nombre = i;
                    }
                    // Modificar para guardar en ruta especifica
                    try
                    {
						descargado = true;
						webClient.DownloadFileAsync(new Uri(e.Message.Text), pathtorrent + Path.DirectorySeparatorChar + nombre); // Mejor utilizar path separator porque distingue entre sistemas
			            // Por algun motivo si es C:\ no deja guardar ahi el archivo habrá que mirar quizas ejecutando como administrador...
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, "👍Torrent <b>" + Uri.UnescapeDataString(nombre) + " </b> añadido.", parseMode: ParseMode.Html);
                    }
                    catch (System.Net.WebException)
                    {
						descargado = false;
                    }
					if (!descargado) 
					{
						await bot.SendTextMessageAsync(e.Message.Chat.Id, "⚠️ La url no es correcta o no es accesible ⚠️", parseMode: ParseMode.Html);
					}
                }
            }
        }

        // Send message
        if (conf.userPropertys(listIndex).isAuth)
        {
            messageToSend = "-----------------------Menú----------------------";
            // Aquí envía el primer teclado inline.
            if (e.Message.Text.Contains("/start") && conf.userPropertys(listIndex).isAuth)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, messageToSend, replyMarkup: keyboard, parseMode: ParseMode.Html);
            }
        }
        else
        {
            // Si no está autorizado todos va por mensajes normales.
            await bot.SendTextMessageAsync(e.Message.Chat.Id, messageToSend, replyMarkup: new ReplyKeyboardHide(), parseMode: ParseMode.Html);
        }

        // Liberar memoria
        messageToSend = null;
        // bot = null;
    }
}