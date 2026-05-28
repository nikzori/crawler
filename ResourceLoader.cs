using System.Text.Json;

public static class ResourceLoader
{
    static string ResourcePath = System.AppContext.BaseDirectory + "/Resources";

    public static List<Item> Items = new();

    // Ways to implement custom objects in the game:
    // [X] JSON deserialization
    // -- can't be extended outside of the source code
    //
    // [✓] Lua implementation
    // -- widely extendable
    // -- common scripting solution for a lot of moddable game
    //
    // [X] C# scripting
    // -- widely extendable
    // -- less learning on my side 
    public static void LoadResources()
    {
        string[] allFiles = Directory.GetFiles(ResourcePath, ".json", SearchOption.AllDirectories);
        foreach (string filepath in allFiles)
        {
            string jsonString = File.ReadAllText(filepath);
            Item? _item = JsonSerializer.Deserialize<Item>(jsonString);
            if (_item is not null)
                Items.Add(_item);
            // I can utilize this: https://learn.microsoft.com/en-us/dotnet/api/system.activator.createinstance?view=net-10.0
            // Alternatively, maybe it's time to look into Lua
        }
    }

}
