using System;

namespace telegramNasBot
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			foreach (string data in args) {
				Console.WriteLine(data);
			}

			Console.ReadLine();
		}
	}
}
