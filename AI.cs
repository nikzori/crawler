public static class AI
{
  static GameObject playerGO = Game.playerGO;
  public static Dictionary<Creature, AIState> creatures = new();

  public static void TestAct(int aut)
  {
    // holy fuck how do I even do this
    foreach (Creature creature in creatures.Keys)
    {
      creature.aut += aut;
      // TODO: list of actions for creatures to cycle through aut costs
      if (creature.aut >= 10)
      {
        Random rng = new Random();
        int x, y;
        while (true)
        {
          x = rng.Next(-1, 2);
          y = rng.Next(-1, 2);
          if ((x != 0 || y != 0) && creature.gameObject.Move(x, y))
            break;
        }        
      }
    }
  }


  public static bool CanSeePlayer(GameObject creatureGO)
  {
    //check whether the Player is in vision range in the first place
    int x = Math.Abs(creatureGO.pos.x - playerGO.pos.x);
    int y = Math.Abs(creatureGO.pos.y - playerGO.pos.y);
    // since our FOV is square shaped (because of equidistant movement), 
    // we don't need to calculate the distance with a square root
    if (x > 10 || y > 10)
      return false;

    else // cast a ray, go through every point until we either reach Player or hit a non-transparent cell
    {
      (int x, int y)[] points = Dungeon.GetLine(creatureGO.pos, playerGO.pos);
      for (int i = 0; i < points.Length; i++)
      {
        if (Game.dungeon.GetCurrentFloor().cells[points[i].x, points[i].y].isTransparent == false)
          return false;
      }
      return true;
    }
  }

  // needs A* or something, idk
  public static bool CanReachPoint(GameObject creatureGO, (int x, int y) pos)
  {
    return false;
  }


  public static bool CanReachAttack(GameObject host, GameObject target)
  {
    return false;
  }
}

// stand in place until sees player
// for now attack state will just change creature rune
public enum AIState { idle, attack, pursuit }
