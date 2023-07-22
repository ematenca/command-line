using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace CommandLine
{
    public static class CommandLineModel
    {
        public static string CurrentDirectory => ConfigurationManager.AppSettings["CurrentDirectory"] ?? Directory.GetCurrentDirectory();

        /// <summary>
        /// Crea un archivo a partir de los argumentos proporcionados.
        /// </summary>
        /// <param name="arguments">Los argumentos para la creación del archivo.</param>
        /// <returns>Un mensaje que indica el resultado de la creación del archivo.</returns>
        public static string CreateFile(string[] arguments)
        {
            string command = arguments[0];

            if (arguments.Length == 1)
                return $"El comando '{command}' debe tener un argumento válido: {command} [nombre de archivo].";

            if (arguments.Length > 2)
                return $"El comando '{command}' solo puede contener un argumento: {command} [nombre de archivo].";

            if (string.IsNullOrEmpty(arguments[1]))
                return $"El nombre del archivo no puede ser vacío.";

            char[] invalidChars = Path.GetInvalidFileNameChars();

            if (invalidChars.Any(item => arguments[1].Contains(item)))
            {
                char invalidChar = invalidChars.First(item => arguments[1].Contains(item));
                return $"Nombre de archivo incorrecto. Caracter inválido: '{invalidChar}'";
            }

            string fileName = arguments[1];
            string filePath = GetFullPath(fileName);

            if (File.Exists(filePath))
                return "El archivo ya existe.";

            using (FileStream fs = new(filePath, FileMode.Create))
                fs.Close();

            return $"Archivo '{fileName}' creado con éxito.";
        }

        /// <summary>
        /// Renombra o mueve un archivo según los argumentos proporcionados.
        /// </summary>
        /// <param name="arguments">Los argumentos para la operación de renombrar o mover el archivo.</param>
        /// <returns>Un mensaje que indica el resultado de la operación.</returns>
        public static string RenameOrMoveFile(string[] arguments)
        {
            string command = arguments[0];

            if (arguments.Length <= 2)
                return $"El comando '{command}' debe tener dos argumentos válidos: {command} [archivo1] [archivo2] o {command} [path1] [path2].";

            if (arguments.Length > 3)
                return $"El comando '{command}' solo puede contener dos argumentos: {command} [archivo1] [archivo2] o {command} [path1] [path2].";

            bool argumentsContainPath = arguments[1].Contains(Path.DirectorySeparatorChar) || arguments[2].Contains(Path.DirectorySeparatorChar);
            string sourceFullPathFile = GetFullPath(arguments[1]);
            string destinationFullPathFile = GetFullPath(arguments[2]);

            if (!File.Exists(sourceFullPathFile))
                return $"El archivo '{Path.GetFileName(sourceFullPathFile)}' no existe.";

            if (!argumentsContainPath)
            {
                File.Move(sourceFullPathFile, destinationFullPathFile);
                return "Archivo renombrado con éxito.";
            }

            string? dir = Path.GetDirectoryName(destinationFullPathFile);

            if (!Directory.Exists(dir))
                return $"El directorio '{dir}' no existe.";

            string sourceFileName = Path.GetFileName(sourceFullPathFile);
            string destinationFileName = Path.GetFileName(destinationFullPathFile);

            if (sourceFileName != destinationFileName)
                return $"El nombre del archivo de destino debe ser igual al de origen. Origen '{sourceFileName}', Destino '{destinationFileName}'.";

            if (File.Exists(destinationFullPathFile))
                return "No es posible mover el archivo a otro directorio, dado que ya existe un archivo con el mismo nombre en esa ubicación.";

            File.Move(sourceFullPathFile, destinationFullPathFile);
            return "Archivo movido con éxito.";
        }

        /// <summary>
        /// Lista los archivos y directorios en el directorio actual normalmente o de forma recursiva, según los argumentos proporcionados.
        /// </summary>
        /// <param name="arguments">Los argumentos para la operación de listado de archivos y directorios.</param>
        /// <returns>Un mensaje que contiene la lista de archivos y directorios.</returns>
        public static string ListFiles(string[] arguments)
        {
            string command = arguments[0];

            if (arguments.Length > 2)
                return $"El comando '{command}' solo puede tener los siguientes argumentos: {command} o {command} -R.";

            if (arguments.Length == 2 && arguments[1].ToLower() != "-r")
                return $"El comando '{command}' debe tener un argumento válido: {command} -R.";

            string[] files = Directory.GetFiles(CurrentDirectory);
            string[] dirs = Directory.GetDirectories(CurrentDirectory);

            if (arguments.Length == 1)
                return ListFiles(CurrentDirectory, files, dirs);

            return ListFilesRecursively(CurrentDirectory, files, dirs);
        }

        /// <summary>
        /// Lista los archivos y directorios en el directorio especificado.
        /// </summary>
        /// <param name="path">La ruta del directorio.</param>
        /// <param name="files">La lista de archivos en el directorio.</param>
        /// <param name="directories">La lista de subdirectorios en el directorio.</param>
        /// <returns>Un mensaje que contiene la lista de archivos y directorios.</returns>
        private static string ListFiles(string path, string[] files, string[] directories)
        {
            StringBuilder message = new();

            if (files.Length > 0)
            {
                message.AppendLine($"{Environment.NewLine}Archivos en {path}:");

                foreach (string file in files)
                    message.AppendLine($"- {Path.GetFileName(file)}");
            }

            directories = directories.OrderByDescending(dir => ContainsFilesOrDirectories(path, dir)).ToArray();

            if (directories.Length > 0)
            {
                message.AppendLine($"{Environment.NewLine}Directorios en {path}:");

                foreach (var dir in directories)
                    message.AppendLine($"+ {Path.GetFileName(dir)}");
            }

            if (files.Length == 0 && directories.Length == 0 && path == CurrentDirectory)
            {
                message.AppendLine($"No se encontraron archivos ni directorios en: '{path}'.");
            }

            return message.ToString().TrimEnd();
        }

        /// <summary>
        /// Lista los archivos y directorios de forma recursiva en el directorio especificado y sus subdirectorios.
        /// </summary>
        /// <param name="path">La ruta del directorio.</param>
        /// <param name="files">La lista de archivos en el directorio.</param>
        /// <param name="directories">La lista de subdirectorios en el directorio.</param>
        /// <returns>Un mensaje que contiene la lista de archivos y directorios.</returns>
        private static string ListFilesRecursively(string path, string[] files, string[] directories)
        {
            string message = ListFiles(path, files, directories);

            foreach (string dir in directories)
            {
                string subFullPath = Path.Combine(path, dir);
                string[] subFiles = Directory.GetFiles(subFullPath);
                string[] subDir = Directory.GetDirectories(subFullPath);

                if (subFiles.Length > 0 || subDir.Length > 0)
                    message += $"{Environment.NewLine}{(ListFilesRecursively(subFullPath, subFiles, subDir))}";
            }

            return message.ToString();
        }

        /// <summary>
        /// Abre el directorio especificado en el explorador de archivos del sistema.
        /// </summary>
        /// <param name="arguments">Los argumentos para la operación de apertura de directorio.</param>
        /// <returns>Un mensaje que indica si el directorio se abrió correctamente.</returns>
        public static string OpenDirectory(string[] arguments)
        {
            string command = arguments[0];

            if (arguments.Length == 1)
                return $"El comando '{command}' debe tener un argumento válido: {command} [path].";

            if (arguments.Length > 2)
                return $"El comando '{command}' solo puede contener un argumento válido: {command} [path].";

            if (string.IsNullOrEmpty(arguments[1]))
                return $"El path especificado no puede ser vacío.";

            if (!Directory.Exists(arguments[1]))
                return $"El directorio '{arguments[1]}' no existe.";

            Process.Start(new ProcessStartInfo { FileName = arguments[1], UseShellExecute = true });
            return "Directorio abierto con éxito.";
        }

        /// <summary>
        /// Este método devuelve un diccionario que contiene los comandos disponibles en la aplicación y su descripción correspondiente.
        /// </summary>
        /// <returns>Un diccionario que mapea cada comando con su descripción.</returns>
        public static Dictionary<string, string> GetCommands()
        {
            return new Dictionary<string, string>
            {
                { "tch [nombre de archivo]", "Crea un archivo nuevo vacío con el siguiente nombre y extensión." },
                { "mv [archivo1] [archivo2]", "Cambia de nombre un archivo." },
                { "mv [path1] [path2]", "Mueve un archivo o directorio." },
                { "ls", "Muestra los archivos/carpetas que se encuentran en el directorio." },
                { "ls -R", "Muestra el contenido de todos los subdirectorios de forma recursiva." },
                { "cd [path]", "Permite navegar entre los diferentes directorios." },
                { "help", "Permite ver un listado de cada comando y su descripción." },
                { "exit", "Finaliza la aplicación."}
            };
        }

        /// <summary>
        /// Verifica si un directorio contiene archivos o subdirectorios.
        /// </summary>
        /// <param name="directory">El nombre del directorio.</param>
        /// <param name="path">La ruta del directorio principal.</param>
        /// <returns>True si el directorio contiene archivos o subdirectorios; de lo contrario, false.</returns>
        private static bool ContainsFilesOrDirectories(string path, string directory)
        {
            string fullPath = Path.Combine(path, directory);
            return Directory.GetFiles(fullPath).Length > 0 || Directory.GetDirectories(fullPath).Length > 0;
        }

        /// <summary>
        /// Obtiene la ruta completa combinando la ruta actual del directorio y una ruta relativa.
        /// </summary>
        /// <param name="path">La ruta relativa a combinar.</param>
        /// <returns>La ruta completa resultante.</returns>
        private static string GetFullPath(string path)
        {
            return Path.Combine(CurrentDirectory, path);
        }
    }
}
