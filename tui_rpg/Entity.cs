/// <summary>
/// Base class for any item or creature.
/// The idea is to pass this to GameObjects for runes and names. May be redundant. 
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
