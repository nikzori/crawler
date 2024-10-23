public class Item : Entity
{
  public string description = "";

  public int price = 1; //for shops
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
}

public class Equipment : Item // kinda redundant
{
  public EquipSlot slot;
}

public class Weapon : Equipment
{
  public bool isTwoHanded = false;
  public bool isRanged = false;
  public int range = 1;
  public float attackSpeed = 1f;
  public int baseDamage = 1;
}

public enum EquipSlot { Head, Neck, Body, Arms, LeftHand, RightHand, Feet, NeckTrinket, RightHandTrinket, LeftHandTrinket }
