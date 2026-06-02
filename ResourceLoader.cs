using System.Text.Json;

public static class ResourceLoader
{
    static string ResourcePath = System.AppContext.BaseDirectory + "/Resources";

    public static List<Item> Items = new();

    public static void LoadResources()
    {
        string[] allFiles = Directory.GetFiles(ResourcePath, ".lua", SearchOption.AllDirectories);
        foreach (string filepath in allFiles)
        {
            string jsonString = File.ReadAllText(filepath);
            Item? _item = JsonSerializer.Deserialize<Item>(jsonString);
            if (_item is not null)
                Items.Add(_item);
        }
    }
}
