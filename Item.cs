public class Item
{
  public string name = "Entity";
  public Rune rune;

  public string description = "";
  public int price = 1;

  public Item(string name, Rune rune, string description, int price)
  {
    this.name = name;
    this.rune = rune;
    this.description = description;
    this.price = price;

  }
}

public class Consumable : Item
{
  int charge = 1; // 
  // idk how to serialize these so I guess the plan for now is to hardcode potions and wands and such

  public void Use()
  {
    if (charge > 0)
    {
      // do stuff
      charge--;
    }
    // the inventory can be populated by all of the consumables and not render those that are at 0 charge
  }
  public Consumable(string name, Rune rune, string description, int price, int charge) : base(name, rune, description, price)
  {
    this.charge = charge;
  }
}

public class Equipment : Item // kinda redundant
{
  public EquipSlot slot;

  public Equipment(string name, Rune rune, string description, int price, EquipSlot slot) : base(name, rune, description, price)
  {
    this.slot = slot;
  }
}

public class Weapon : Equipment
{
  public bool isTwoHanded = false;
  public bool isRanged = false;
  public int range = 1;
  public float attackSpeed = 1f;
  public int baseDamage = 1;

  public Weapon(string name, Rune rune, string description, int price, EquipSlot slot, bool isTwoHanded, bool isRanged, int range, float attackSpeed, int baseDamage) : base(name, rune, description, price, slot)
  {
    this.isTwoHanded = isTwoHanded;
    this.isRanged = isRanged;
    this.range = range;
    this.attackSpeed = attackSpeed;
    this.baseDamage = baseDamage;
  }
}

public class Armor : Equipment
{
  public int armor = 0;
  public int encumbrance = 0;

  public Armor(string name, Rune rune, string description, int price, EquipSlot slot, int armor, int encumbrance) : base(name, rune, description, price, slot)
  {
    this.armor = armor;
    this.encumbrance = encumbrance;
  }
}

public enum EquipSlot { Head, Neck, Body, Arms, MainHand, OffHand, Feet, NeckTrinket, RightHandTrinket, LeftHandTrinket }
