using System.Text;

namespace CommandLine
{
    public class CommandLineController
    {
        /// <summary>
        /// Inicia el bucle principal de la aplicación de línea de comandos.
        /// Muestra el mensaje de bienvenida y la ayuda inicial.
        /// Lee los comandos de entrada del usuario y ejecuta las operaciones correspondientes.
        /// </summary>
        public void Start()
        {
            CommandLineView.DisplayWelcomeMessage();
            CommandLineView.DisplayHelp(CommandLineModel.GetCommands());
            CommandLineView.DisplayMessage($"{Environment.NewLine}* Si desea utilizar espacios en los nombres de archivos o rutas, inserte los mismos entre comillas.");

            bool exit = false;
            while (!exit)
            {
                try
                {
                    CommandLineView.DisplayPrompt();
                    string? input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input))
                    {
                        CommandLineView.DisplayMessage("Comando no reconocido. Escribe 'help' para ver los comandos disponibles.");
                        continue;
                    }

                    string[] arguments = SplitInputBySpacesAndQuotes(input);
                    string command = arguments[0].ToLower();

                    string message = string.Empty;
                    switch (command)
                    {
                        case "tch":
                            message = CommandLineModel.CreateFile(arguments);
                            break;
                        case "mv":
                            message = CommandLineModel.RenameOrMoveFile(arguments);
                            break;
                        case "ls":
                            message = CommandLineModel.ListFiles(arguments);
                            break;
                        case "cd":
                            message = CommandLineModel.OpenDirectory(arguments);
                            break;
                        case "exit":
                            exit = true;
                            break;
                        case "help":
                            CommandLineView.DisplayHelp(CommandLineModel.GetCommands());
                            CommandLineView.DisplayMessage($"{Environment.NewLine}* Si desea utilizar espacios en los nombres de archivos o rutas, inserte los mismos entre comillas.");
                            continue;
                        default:
                            CommandLineView.DisplayMessage("Comando no reconocido. Escribe 'help' para ver los comandos disponibles.");
                            continue;
                    }

                    CommandLineView.DisplayMessage(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Este método divide una cadena de entrada en elementos separados por espacios, teniendo en cuenta las comillas para mantener elementos entre comillas como una sola unidad.
        /// </summary>
        /// <param name="input">La cadena de entrada que se va a dividir.</param>
        /// <returns>Un arreglo de cadenas que contiene los elementos obtenidos después de dividir la cadena de entrada.</returns>
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
