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
            conf.setKey(token);
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
        bool _isTokenOk = false;
        Console.Write("Checking bot token... ");
        if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
        {
            text.writeWithColor("Fail. ", ConsoleColor.Red, true);
            do {
                token = createToken ();
            } while (!this.checkToken (token));
            _isTokenOk = true;
        } else {
            if (System.Text.RegularExpressions.Regex.IsMatch(token, @"^\d*:[\w\d-_]{35}$")) 
            {
                text.writeWithColor("Ok. ", ConsoleColor.DarkGreen, true);
                this.botToken = token;
                _isTokenOk = true;
            }
        }
        return _isTokenOk;
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
            conf.setPW (passWord);
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
            text.writeWithColor("Ok. ", ConsoleColor.DarkGreen, true);
            this.authPass = passWord;
            return true;
        }
    }

    public bool isLinux () {
        int p = (int)System.Environment.OSVersion.Platform;
        return (p == 4) || (p == 6) || (p == 128);
    }
}

