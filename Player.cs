public class Player : Creature
{
  public int sightRadius = 10;

  public List<Item> inventory = new();

  public Player(string name, (int x, int y) pos, Rune rune) : base(name, pos, rune)
  {
  }

  public void TileInteract((int x, int y) pos) { TileInteract(pos.x, pos.y); }

  public void TileInteract(int x, int y)
  {
    int xt = pos.x + x;
    int yt = pos.y + y;

    if (Game.currentMap.cells[xt, yt].creature is null)
      this.MoveTo(xt, yt);
    else Game.currentMap.cells[xt, yt].creature.ReceiveDamage(10f);

    Game.Update(10);
  }

}
