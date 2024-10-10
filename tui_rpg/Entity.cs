/// <summary>
/// Base class for any item or creature.
/// </summary>
public class Entity
{
  public string name = "Entity";
  public Rune rune;

  public Entity()
  {
    rune = new('ж');
  }
  public Entity(Rune r)
  {
    rune = r;
  }
}
