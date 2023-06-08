using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CommandLine
{
    public static class CommandLineModel
    {
        private static readonly Regex regex = new("[^a-zA-Z\x2e\x5f\x20\x2d]");

        public static string CurrentDirectory
        {
            get { return ConfigurationManager.AppSettings["CurrentDirectory"] ?? Directory.GetCurrentDirectory(); }
        }

        public static void CreateFile(string[] arguments)
        {
            string command = arguments[0];
            if (arguments.Length <= 1)
            {
                Console.WriteLine($"El comando '{command}' debe tener un argumento válido: {command} [nombre de archivo].");
                return;
            }

            if (regex.IsMatch(arguments[1]))
            {
                Console.WriteLine($"El nombre del archivo solo puede contener caracteres alfabéticos.");
                Console.WriteLine($"Y los siguientes caracteres: '-' (guion), '_' (guion bajo), '.' (punto), ' ' (espacio).");
                return;
            }

            string fileName = arguments[1];
            string filePath = GetFullPath(fileName);

            if (File.Exists(filePath))
            {
                Console.WriteLine("El archivo ya existe.");
                return;
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                fs.Close();

            Console.WriteLine($"Archivo '{fileName}' creado con éxito.");
        }

        public static void RenameOrMoveFile(string[] arguments)
        {
            string command = arguments[0];
            if (arguments.Length != 3)
            {
                Console.WriteLine($"El comando '{command}' debe tener dos argumentos válidos: {command} [archivo1] [archivo2] o {command} [path1] [path2].");
                return;
            }

            bool argumentsContainsPath = arguments[1].Contains(Path.DirectorySeparatorChar) || arguments[2].Contains(Path.DirectorySeparatorChar);
            string sourceFullPathFile = GetFullPath(arguments[1]);
            string destinationFullPathFile = GetFullPath(arguments[2]);

            if (!File.Exists(sourceFullPathFile))
            {
                Console.WriteLine($"El archivo '{Path.GetFileName(sourceFullPathFile)}' no existe.");
                return;
            }

            if (!argumentsContainsPath)
            {
                File.Move(sourceFullPathFile, destinationFullPathFile);
                Console.WriteLine("Archivo renombrado con éxito.");
                return;
            }

            string? dir = Path.GetDirectoryName(destinationFullPathFile);
            if (!Directory.Exists(dir))
            {
                Console.WriteLine($"El directorio '{dir}' no existe.");
                return;
            }

            string sourceFileName = Path.GetFileName(sourceFullPathFile);
            string destinationFileName = Path.GetFileName(destinationFullPathFile);

            if (sourceFileName != destinationFileName)
            {
                Console.WriteLine($"El nombre del archivo de destino debe ser igual al de origen. Origen '{sourceFileName}', Destino '{destinationFileName}'.");
                return;
            }

            if (File.Exists(destinationFullPathFile))
            {
                Console.WriteLine("No es posible mover el archivo a otro directorio, dado que ya existe un archivo con el mismo nombre en esa ubicación.");
                return;
            }

            File.Move(sourceFullPathFile, destinationFullPathFile);
            Console.WriteLine("Archivo movido con éxito.");
        }

        public static void ListFiles(string[] arguments)
        {
            string[] files = Directory.GetFiles(CurrentDirectory);
            string[] subDir = Directory.GetDirectories(CurrentDirectory);
            string command = arguments[0];

            if (arguments.Length == 1 && arguments[0] == "ls")
            {
                ListFiles(CurrentDirectory, files, subDir);
            }
            else if(arguments.Length == 2 && arguments[1] == "-R")
            {
                ListFilesRecursively(CurrentDirectory, files, subDir);
            }
            else
            {
                Console.WriteLine($"El comando '{command}' debe tener un argumento válido: {command} o {command} -R.");
                return;
            }
        }

        private static void ListFiles(string path, string[] files, string[] subDirectories)
        {
            Console.WriteLine($"Archivos en {path}:");

            foreach (string file in files)
                Console.WriteLine($"- {Path.GetFileName(file)}");

            Console.WriteLine($"Directorios en {path}:");

            foreach (var dir in subDirectories)
                Console.WriteLine($"- {Path.GetFileName(dir)}");
        }

        private static void ListFilesRecursively(string path, string[] files, string[] subDirectories)
        {
            ListFiles(path, files, subDirectories);

            if (subDirectories.Length <= 0)
                Console.WriteLine($"+ No existen directorios.");

            foreach (string dir in subDirectories)
            {
                string subDirFullPath = Path.Combine(path, dir);
                string[] subFiles = Directory.GetFiles(subDirFullPath);
                string[] subDir = Directory.GetDirectories(dir);

                ListFilesRecursively(subDirFullPath, subFiles, subDir);
            }
        }

        public static void OpenDirectory(string[] arguments)
        {
            string command = arguments[0];
            if (arguments.Length <= 1)
            {
                Console.WriteLine($"El comando '{command}' debe tener un argumento válido: {command} [path].");
                return;
            }

            if (!Directory.Exists(arguments[1]))
            {
                Console.WriteLine($"El directorio '{arguments[1]}' no existe.");
                return;
            }

            Process.Start(new ProcessStartInfo { FileName = arguments[1], UseShellExecute = true });
            return;
        }

        private static string GetFullPath(string path)
        {
            return Path.Combine(CurrentDirectory, path);
        }

        public static Dictionary<string, string> GetCommands()
        {
            return new Dictionary<string, string>
            {
                { "tch [nombre de archivo]", "Crea un archivo nuevo vacío con el siguiente nombre y extensión." },
                { "mv [archivo1] [archivo2]", "Cambia de nombre un archivo" },
                { "mv [path1] [path2]", "Mueve un archivo o directorio" },
                { "ls", "Muestra los archivos/carpetas que se encuentran en el directorio" },
                { "ls -R", "Muestra el contenido de todos los subdirectorios de forma recursiva" },
                { "cd [path]", "Permite navegar entre los diferentes directorios" },
                { "help [comando]", "Permite ver un listado de cada comando y su descripción" },
                { "exit", "Finaliza la aplicación"}
            };
        }
    }
}
