using System;

public class consoleTweaks {
	
	public void writeWithColor(string text, ConsoleColor color = ConsoleColor.White, bool newLine = false) {
		if (newLine) {
			Console.ForegroundColor = color;
			Console.WriteLine (text);
			Console.ResetColor ();
		} else {
			Console.ForegroundColor = color;
			Console.Write (text);
			Console.ResetColor ();
		}
	}	
}