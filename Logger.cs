// The Logger class is used to log program details.
// It's neccecary to have since stdout is used for the game.

using System;

static class Logger
{
    // The folder where the logs are stored
    private static string logFolder = "logs";

    private static void EnsureLogFolderExists()
    {
        // If the log folder doesn't exist, create it
        if (!Directory.Exists(logFolder))
        {
            Directory.CreateDirectory(logFolder);
        }
    }

    // Writes a debug statement - these messages are not neccesarilly 
    // errors or warnings, but can be used to debug the program
    public static void Debug(string line)
    {
        // Ensure the log folder exists
        EnsureLogFolderExists();

        // Figure out which thread is writing to the log
        string thread = Thread.CurrentThread.Name ?? "UknownThread";

        StreamWriter debugFile = new StreamWriter($"{logFolder}/debug-{thread}.txt", true);

        // Add a timestamp and append
        debugFile.WriteLine($"{DateTime.Now} {line}");
        debugFile.Close();
    }

    // Writes an error to the error file
    public static void Error(string line)
    {
        // Ensure the log folder exists
        EnsureLogFolderExists();


        // Figure out which thread is writing to the log
        string thread = Thread.CurrentThread.Name ?? "UknownThread";

        StreamWriter errorFile = new StreamWriter($"{logFolder}/error-{thread}.txt", true);

        // Add a timestamp to the line
        errorFile.WriteLine($"{DateTime.Now} {line}");

        // Write the error to the console
        Console.Error.WriteLine($"{DateTime.Now} {line}");
    }

}