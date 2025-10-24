using Terminal.Gui.App;
using Terminal.Gui.Configuration;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
ConfigurationManager.Enable(ConfigLocations.All);

Application.Run<MainMenu>().Dispose();
Application.Shutdown();

static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Exception? ex = e.ExceptionObject as Exception;
    string LogFilePath = "/home/zori/Project/crawler/Error.log";
    try
    {
        File.WriteAllText(LogFilePath, $"[{DateTime.Now}] Unhandled exception: {ex?.Message}\nStack trace: {ex?.StackTrace}\n");
    }
    catch (Exception logEx)
    {
        Console.WriteLine($"Error logging unhandled exception: {logEx.Message}\n");
    }
}

Console.WriteLine("Bye-bye!~");
