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
    public int Price { get; init; }

    public Item(string name = "Item", string description = "It's uuuh... It's a- it's the, um...", int price = 1)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
    }
}
public enum EquipmentSlot {Head, Body, Feet, Arms, MainHand, OffHand}
public class Equippable : Item 
{
    public int BaseValue { get; init; } 
    public List<EquipmentSlot> Slot = new();
    public Dictionary<object, int> Modifiers = new();
    public int GetModifiers() 
    {
        int result = 0;
        foreach (KeyValuePair<object, int> kvp in Modifiers)
          result += kvp.Value;
        return result;
    }

    public int Value { get => BaseValue + GetModifiers(); }
}
public class Weapon : Equippable
{
  public int BaseDamage { get; init; }
  public int Range {get; init; }
  public DamageType Type { get; }
  public Action OnHit { get; } 
  public Weapon(Action? onHit, string name = "Weapon", string description = "Whatever it is, it hits well enough.", int price = 1, int baseDamage = 1, DamageType damageType = DamageType.Physical) 
  {
    this.BaseDamage = baseDamage;
    this.Type = damageType;
    if (onHit is not null)
        OnHit = onHit;
    else OnHit = () => { return; };
  }
}

public enum DamageType { Physical, Fire, Ice, Electric, True };
