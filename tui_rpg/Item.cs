public class Item : Entity
{
  public string description = "";

  public int price = 1; //for shops

  public Item(string name, Rune rune, string description, int price) : base(name, rune)
  {
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
    // do stuff
    charge--;

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
  public int encumberence = 0;

  public Armor(string name, Rune rune, string description, int price, EquipSlot slot, int armor, int encumberence) : base(name, rune, description, price, slot)
  {
    this.armor = armor;
    this.encumberence = encumberence;
  }
}

public enum EquipSlot { Head, Neck, Body, Arms, LeftHand, RightHand, Feet, NeckTrinket, RightHandTrinket, LeftHandTrinket }
