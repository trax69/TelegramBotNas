using System;
using Telegram.Bot;

public class appCheck
{
    appConfig conf = new appConfig();
    consoleTweaks text = new consoleTweaks();

    // Variables
    public string botToken;
    public string authPass;

    /* Crear el token si no existe */
    private string createToken()
    {
        Console.WriteLine("Couldn't detect bot Token, please write it now: ");
        string token = Console.ReadLine();
        Console.Write("Is the token correct ? (Y/N): ");
        // Comprobar que ha escrito la Y
        if (Console.ReadKey().Key == ConsoleKey.Y) {
            Console.WriteLine(" ");
            conf.saveKey(token);
            // Una vez creado el token comprobar si es correcto
            return token;
        } else {
            Console.WriteLine(" ");
            return null;
        }
    }

    /* Metodo para comprobar que el token es correcto
		Devuelve el mismo token si es válido o el token creado
	*/
    public bool checkToken(string token)
    {
        Console.Write("Checking bot token... ");
        if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
        {
            text.writeWithColor("Fail. ", ConsoleColor.Red, true);
            do {
                token = createToken ();
            } while (this.checkToken (token) != true);
            return true;
        }
        else
        {
            try {
                var bot = new TelegramBotClient(token);
                if (bot.TestApiAsync().Result) {
                    text.writeWithColor("Ok. ", ConsoleColor.DarkGreen, true);
                }
                this.botToken = token;
                return true;
            } catch (ArgumentException) {
                text.writeWithColor("Token not valid. ", ConsoleColor.Red, true);
                return false;
            }
        }
    }

    /* Crear la contraseña si no existe */
    public string createAuth()
    {
        Console.WriteLine("Couldn't detect auth password, please write it now: ");
        string passWord = Console.ReadLine();
        Console.Write("Is the password correct ? (Y/N): ");
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            Console.WriteLine(" ");
            conf.savePW (passWord);
            this.authPass = passWord;
            return passWord;
        } else {
            Console.WriteLine(" ");
            return null;
        }
    }

    /* Metodo para comprobar que la contraseña es correcta
		Devuelve la misma contraseña si es válida o la constraseña creada
	*/
    public bool checkAuth(string passWord)
    {
        Console.Write("Checking auth password... ");
        if (string.IsNullOrEmpty(passWord) || string.IsNullOrWhiteSpace(passWord))
        {
            // Añadidos colorines al Fail.
            text.writeWithColor("Fail. ", ConsoleColor.Red, true);
            do {
                passWord = createAuth();
            } while (this.checkAuth(passWord) != true);
            return true;
        } else {
            // Añadidos colorines al OK.
            text.writeWithColor("Ok. ", ConsoleColor.DarkGreen);
            Console.Write("Password is ");
            text.writeWithColor(passWord, ConsoleColor.DarkGreen, true);
            this.authPass = passWord;
            return true;
        }
    }
}

