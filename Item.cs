public class Inventory
{
    public Dictionary<Item, int> items = new();
    public float TotalWeight
    {
        get
        {
            float res = 0f;
            foreach (KeyValuePair<Item, int> kvp in items)
                res += kvp.Key.Weight * kvp.Value;
            return res;
        }
    }
    public bool AddItem(Item item)
    {
        return true;
    }
    public bool RemoveItem(Item item)
    {
        return true;
    }

}

public class Item
{
    public string Name { get; }

    public string Description { get; }
    public int Price { get; }
    public float Weight { get; }

    public Item(string name = "Item", string description = "It's uuuh... It's a- it's the, um...", int price = 1, float weight = 0.2f)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Weight = weight;
    }
}

public interface IAttack
{
    int BaseDamage { get; }
    void GetDamageValue();
}

