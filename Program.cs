using Terminal.Gui.App;
using Terminal.Gui.Views;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

using (var app = Application.Create().Init())
{
    MainMenu mainMenu = new MainMenu();
    app.Run(mainMenu);
    mainMenu.Dispose();
}

Console.WriteLine("Bye-bye!~");

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

