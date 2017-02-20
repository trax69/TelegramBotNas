using System;
using Telegram.Bot;

public class appCheck {

	string botKey;
	string pW;
	appConfig conf = new appConfig();

	/* Metodo para comprobar que el token es correcto */
	public bool checkToken(string token)
	{
		Console.Write("Checking bot token... ");
		// Probar si el token no es nulo y si contiene ':'
		if (token != null && token.Contains(":"))
		{
			try
			{
				var bot = new TelegramBotClient(token);
				if (bot.TestApiAsync().Result)
				{
					// Añadidos colorines al OK.
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine("OK.");
					Console.ResetColor();
					return true;
				}
			}
			catch (System.ArgumentException)
			{
				checkToken(null);
				return false;
			}
		}
		else
		{
			// Añadidos colorines al Fail.
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Fail.");
			Console.ResetColor();

			Console.WriteLine("Couldn't detect bot Token, please write it now: ");
			botKey = Console.ReadLine();
			Console.Write("Is the token correct ? (Y/N): ");
			// Comprobar que ha escrito la Y
			if (Console.ReadKey().Key == ConsoleKey.Y)
			{
				Console.Write(" ");
				conf.saveSettings(botKey);

				// Añadir colorines :D
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("Saved !");
				Console.ResetColor();

				checkToken(botKey);
				return false;
			}
			else
			{
				checkToken(null);
				return false;
			}
		}
		return false;
	}

	public bool checkAuth(string passWord)
	{
		Console.Write("Checking auth password... ");
		if (passWord == null)
		{
			// Añadidos colorines al Fail.
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Fail.");
			Console.ResetColor();

			Console.WriteLine("Couldn't detect auth password, please write it now: ");
			pW = Console.ReadLine();
			Console.Write("Is the password correct ? (Y/N): ");
			if (Console.ReadKey().Key == ConsoleKey.Y)
			{
				Console.Write(" ");
				conf.saveSettings(pW);

				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("Saved !");
				Console.ResetColor();

				checkAuth(pW);
				return false;
			}
			else
			{
				checkAuth(null);
				return false;
			}
		}
		else
		{
			// Añadidos colorines al OK.
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.Write("OK. ");
			Console.ResetColor();

			Console.Write("Password is ");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine(pW);
			Console.ResetColor();
			return true;
		}
	}
}

