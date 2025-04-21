public class Player : Creature
{
  public int sightRadius = 10;
  public Player(string name, (int x, int y) pos, Rune rune) : base(name, pos, rune) { }
}
