using System.Text;

public class Container : IInteractable
{
    public string Name { get; }
    public Rune Rune { get; } = new Rune('[');

    public Dictionary<Item, int> Items; // rework, can't use non-static ref types as keys

    public Container(Rune rune, string name = "Crate")
    {
        Name = name;
        Rune = rune;
        Items = new();
    }

    public bool TryInsert(Item item)
    {
        return false;
    }

    public void OnInteract(object? sender, Vector2Int pos)
    {
        // call inventory view
    }
}

