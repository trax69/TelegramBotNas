using System;

public class consoleTweaks {
	
	public void writeWithColor(string text, ConsoleColor color = ConsoleColor.White, bool newLine = false) {
		if (newLine) {
			Console.ForegroundColor = color;
            Console.WriteLine (text.PadLeft(text.Length + 1));
			Console.ResetColor ();
		} else {
			Console.ForegroundColor = color;
            Console.Write (text.PadLeft(text.Length + 1));
			Console.ResetColor ();
		}
	}

    public void centerText (string text) {
        Console.WriteLine (String.Format ("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", text));
    }

    public void centerTextWithColor (string text, ConsoleColor color = ConsoleColor.White) {
        Console.ForegroundColor = color;
        this.centerText (text);
        Console.ResetColor ();
    }

    public void Title(string text, char decoration, ConsoleColor color = ConsoleColor.White) {
        string titleChar = new string (decoration, text.Length + 4);
        centerTextWithColor (titleChar, color);
        text = decoration + " " + text + " " + decoration;  // Añadir la decoración al titulo también
        centerTextWithColor (text, color);
        centerTextWithColor (titleChar, color);
        Console.WriteLine (" ");    // Añadir una linea en blanco para qué quede bien
    }

    public void writeInstructions () {
        
    }
}