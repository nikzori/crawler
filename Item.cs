using System.Diagnostics;

public class Inventory
{
    public Dictionary<Item, int> items = new();
    public void AddItem(Item item, int count = 1)
    {
        if (items.ContainsKey(item))
            items[item] += count;
        else items.Add(item, count);
    }
    public void RemoveItem(Item item, int count = 1)
    {
        if (items.ContainsKey(item) && items[item] >= count)
            items[item] -= count;
        else Debug.WriteLine("Trying to remove item " + item.Name + " from Player's inventory, but it doesn't exist or the count is too low.");
    }

}

public class Item
{
    public string Name { get; }

    public string Description { get; }
    public int Price { get; }

    public Item(string name = "Item", string description = "It's uuuh... It's a- it's the, um...", int price = 1)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
    }
}

public interface IAttack
{
    int BaseDamage { get; }
    void GetDamageValue();
}

