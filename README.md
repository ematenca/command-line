Se desarrolló una aplicación de consola que simula una línea de comandos utilizando .NET Core 6.

La aplicación cuenta con los siguientes comandos:

• tch [nombre de archivo]: Crea un archivo nuevo vacío con el nombre y extensión especificados.

• mv [archivo1] [archivo2]: Cambia el nombre de un archivo.

• mv [path1] [path2]: Mueve un archivo a otro directorio.

• ls: Muestra los archivos/carpetas en el directorio actual.

• ls -R: Muestra el contenido de todos los subdirectorios de forma recursiva.

• cd [path]: Permite navegar entre los diferentes directorios.

• exit: Finaliza la aplicación.

• help: Muestra una lista de comandos disponibles y su descripción.

Se aplicó el patrón de diseño MVC, asignando a cada clase la responsabilidad correspondiente dentro de esta arquitectura.

Para configurar correctamente la aplicación y utilizarla adecuadamente, se debe especificar una ruta completa en el valor correspondiente a la clave CurrentDirectory en el archivo App.Config, ubicado en la carpeta ConfigFiles.
