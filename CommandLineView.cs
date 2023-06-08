namespace CommandLine
{
    public static class CommandLineView
    {
        public static void DisplayWelcomeMessage()
        {
            Console.Write("Bienvenido a la aplicación de línea de comandos.");
            Console.WriteLine(Environment.NewLine);
        }

        public static void DisplayHelp(Dictionary<string, string> commands)
        {
            Console.Write("Comandos disponibles:");
            Console.WriteLine(Environment.NewLine);

            foreach (var command in commands)
            {
                Console.WriteLine($"- {command.Key}: {command.Value}");
            }
        }

        public static void DisplayErrorMessage(string message)
        {
            Console.WriteLine($"Error: {message}");
        }
    }
}
