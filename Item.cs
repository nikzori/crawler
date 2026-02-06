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
    public int Damage { get; }
    public int Range { get; }
    public Weapon(string name = "Weapon", string description = "I think it's a stick?", int price = 1, int damage = 1, int range = 1) : base(name, description, price)
    {
        this.Damage = damage;
        this.Range = range;
    }
}
