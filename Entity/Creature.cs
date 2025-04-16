public class Creature : Entity
{

  public int aut = 0;
  public static int sightRadius = 10;

  public int level = 1;
  public int health = 10;

  public Creature(GameObject gameObject, string name, Rune rune, int level, int health) : base(gameObject, name, rune)
  {
    this.level = level;
    this.health = health;
  }
}

