using Terminal.Gui.App;
using Terminal.Gui.Configuration;

ConfigurationManager.Enable(ConfigLocations.All);

Application.Run<MainMenu>().Dispose();
Application.Shutdown();

Console.WriteLine("Bye-bye!~");
