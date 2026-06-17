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

// This probably needs to be redone to work with some sort of deepcopy method to dupe item objects
// Structs can probably work, too 
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

public class Weapon : Item 
{
  public int BaseDamage { get; }
  public DamageType Type { get; }
  public Action OnHit { get; } 
  public Weapon(Action? onHit, string name = "Weapon", string description = "Whatever it is, it hits well enough.", int price = 1, int baseDamage = 1, DamageType damageType = DamageType.Physical) : base(name, description, price) 
  {
    this.BaseDamage = baseDamage;
    this.Type = damageType;
    if (onHit is not null)
        OnHit = onHit;
    else OnHit = () => { return; };
  }
}

public enum DamageType { Physical, Fire, Ice, Electric, True };
