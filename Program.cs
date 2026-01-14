using System.Runtime.InteropServices;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.Drivers;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

using (IApplication app = Application.Create())
{
    /*
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        app.Init(driverName: DriverRegistry.Names.UNIX);
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
        app.Init(driverName: DriverRegistry.Names.WINDOWS);
    else app.Init(driverName: DriverRegistry.Names.ANSI); 
    //app.Init(driverName: DriverRegistry.Names.DOTNET); 
    */
    app.Init();
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
    catch (Exception logEx) { Console.WriteLine($"Error logging unhandled exception: {logEx.Message}\n"); }
    finally { Console.WriteLine($"[{DateTime.Now}] Unhandled exception: {ex?.Message}\nStack trace: {ex?.StackTrace}"); }
}

