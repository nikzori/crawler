public class Creature : Entity
{

  public int aut = 0;

  public int Somatics = 1,
              Cognition = 1,
              Reflexes = 1,
              Willpower = 1;

  public int level = 1;
  public int sightRadius = 10;
  public int healthMax { get { return 5 * level + 3 * Somatics; } }
  public float health = 1;
  public float healthRegen = 0.2f;

  public int manaMax { get { return 3 * level + 3 * Cognition; } }
  public float mana = 1;
  public float manaRegen = 0.5f;

  public float movementSpeed { get { return 0.5f + (2 / Reflexes); } }
  public List<Item> inventory = new();
  public Dictionary<EquipSlot, Item> equipment = new();
  public Creature(GameObject gameObject, string name, Rune rune, int Somatics = 1, int Cognition = 1, int Reflexes = 1, int Willpower = 1, int level = 1, bool isAIControlled = true) : base(gameObject, name, rune)
  {
    this.gameObject = gameObject;
    this.name = name;
    this.rune = rune;

    this.Somatics = Somatics;
    this.Cognition = Cognition;
    this.Reflexes = Reflexes;
    this.Willpower = Willpower;

    this.level = level;

    if (isAIControlled)
      AI.creatures.Add(this, AIState.idle);
  }
}
