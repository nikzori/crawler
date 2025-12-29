using System.Diagnostics;
using Terminal.Gui.App;
using Terminal.Gui.Input;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
using (var app = Application.Create().Init())
{
    Application.QuitKey = Key.Q.WithCtrl;

    MainWindow MainWindow = new();
    app.Run(MainWindow);
    MainWindow.Dispose();
}

Console.WriteLine("Bye-bye!~");

static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Exception? ex = e.ExceptionObject as Exception;
    string LogFilePath = "/home/zori/Project/crawler/Error.log";
    try { File.WriteAllText(LogFilePath, $"[{DateTime.Now}] Unhandled exception: {ex?.Message}\nStack trace: {ex?.StackTrace}\n"); }
    catch (Exception logEx) { Console.WriteLine($"Error logging unhandled exception: {logEx.Message}\n");}
    finally {Console.WriteLine($"[{DateTime.Now}] Unhandled exception: {ex?.Message}\nStack trace: {ex?.StackTrace}");}
}

