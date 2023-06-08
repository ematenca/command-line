using System.Text;

namespace CommandLine
{
    public class CommandLineController
    {
        public void Start()
        {
            CommandLineView.DisplayWelcomeMessage();
            CommandLineView.DisplayHelp(CommandLineModel.GetCommands());

            bool exit = false;
            while (!exit)
            {
                try
                {
                    Console.Write(Environment.NewLine);
                    Console.Write("> ");
                    string? input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input))
                    {
                        CommandLineView.DisplayErrorMessage("Comando no reconocido. Escribe 'help' para ver los comandos disponibles.");
                        continue;
                    }

                    string[] arguments = SplitInputBySpacesAndQuotes(input);

                    string? command = arguments.Length <= 3 ? arguments[0].ToLower() : null;
                    switch (command)
                    {
                        case "tch":
                            CommandLineModel.CreateFile(arguments);
                            break;
                        case "mv":
                            CommandLineModel.RenameOrMoveFile(arguments);
                            break;
                        case "ls":
                            CommandLineModel.ListFiles(arguments);
                            break;
                        case "cd":
                            CommandLineModel.OpenDirectory(arguments);
                            break;
                        case "help":
                            CommandLineView.DisplayHelp(CommandLineModel.GetCommands());
                            break;
                        case "exit":
                            exit = true;
                            break;
                        default:
                            CommandLineView.DisplayErrorMessage("Comando no reconocido. Escribe 'help' para ver los comandos disponibles.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        static string[] SplitInputBySpacesAndQuotes(string input)
        {
            List<string> pathElements = new List<string>();
            StringBuilder currentElement = new StringBuilder();
            bool insideQuotes = false;

            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (currentChar == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (currentChar == ' ' && !insideQuotes)
                {
                    pathElements.Add(currentElement.ToString());
                    currentElement.Clear();
                }
                else
                {
                    currentElement.Append(currentChar);
                }
            }

            pathElements.Add(currentElement.ToString());

            return pathElements.ToArray();
        }
    }
}
