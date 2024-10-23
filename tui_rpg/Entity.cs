/// <summary>
/// Base class for any item or creature.
/// </summary>
public class Entity
{
  public string name = "Entity";
  public Rune rune;
  public Entity(string name, Rune rune)
  {
    this.name = name;
    this.rune = rune;
  }
}
