using System;
using Telegram.Bot;

public class appCheck
{

    string botKey;
    string pW;
    appConfig conf = new appConfig();
    consoleTweaks text = new consoleTweaks();

    public string getToken()
    {
        return this.botKey;
    }

    public string getpW()
    {
        return this.pW;
    }

    /* Crear el token si no existe */
    private string createToken()
    {
        Console.WriteLine("Couldn't detect bot Token, please write it now: ");
        this.botKey = Console.ReadLine();
        Console.Write("Is the token correct ? (Y/N): ");
        // Comprobar que ha escrito la Y
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            Console.Write(" ");
            conf.setSetting("key", this.botKey);
            // Una vez creado el token comprobar si es correcto
            return this.botKey;
        }
        else {
            return null;
        }
    }

    /* Metodo para comprobar que el token es correcto
		Devuelve el mismo token si es válido o el token creado
	*/
    public bool checkToken(string token)
    {
        Console.Write("Checking bot token... ");
        // Probar si el token no es nulo y si contiene ':'
        if (token.Contains(":"))
        {
            try
            {
                var bot = new TelegramBotClient(token);
                if (bot.TestApiAsync().Result)
                {
                    // Añadidos colorines al OK.
                    text.writeWithColor("Ok. ", ConsoleColor.DarkGreen, true);
                    return true;
                }
            }
            catch (System.ArgumentException)
            {
                checkToken(token);
                return false;
            }
        }
        else if (string.IsNullOrEmpty(token))
        {
            // Poner color rojo al texto
            text.writeWithColor("Fail. ", ConsoleColor.Red, true);
            this.botKey = createToken();
            return false;
        }
        return false;
    }

    /* Crear la contraseña si no existe */
    public string createAuth()
    {
        Console.WriteLine("Couldn't detect auth password, please write it now: ");
        this.pW = Console.ReadLine();
        Console.Write("Is the password correct ? (Y/N): ");
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            Console.Write(" ");
            conf.setSetting("pW", this.pW);
            return this.pW;
        }
        else {
            return null;
        }
    }

    /* Metodo para comprobar que la contraseña es correcta
		Devuelve la misma contraseña si es válida o la constraseña creada
	*/
    public bool checkAuth(string passWord)
    {
        Console.Write("Checking auth password... ");
        if (string.IsNullOrEmpty(passWord))
        {
            // Añadidos colorines al Fail.
            text.writeWithColor("Fail. ", ConsoleColor.Red, true);
            this.pW = createAuth();
            return false;
        }
        else {
            // Añadidos colorines al OK.
            text.writeWithColor("Ok. ", ConsoleColor.DarkGreen);

            Console.Write("Password is ");
            text.writeWithColor(passWord, ConsoleColor.DarkGreen, true);
            return true;
        }
    }
}

