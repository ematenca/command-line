namespace CommandLine
{
    public static class CommandLineView
    {
        public static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Bienvenido a la aplicación de línea de comandos.");
            Console.WriteLine();
        }

        public static void DisplayPrompt()
        {
            Console.WriteLine();
            Console.Write("> ");
        }

        public static void DisplayHelp(Dictionary<string, string> commands)
        {
            Console.WriteLine($"Comandos disponibles:");
            Console.WriteLine();

            foreach (var command in commands)
            {
                Console.WriteLine($"- {command.Key}: {command.Value}");
            }
        }

        public static void DisplayMessage(string message)
        {
            Console.WriteLine($"{message}");
        }
    }
}
