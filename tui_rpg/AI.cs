public static class AI
{
  static GameObject playerGO = Game.playerGO;

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
      (int x, int y)[] points = Map.GetLine(creatureGO.pos, playerGO.pos);
      for (int i = 0; i < points.Length; i++)
      {
        if (Game.map.cells[points[i].x, points[i].y].isTransparent == false)
          return false;
      }
      return true;
    }
  }

  public static void Act(GameObject creatureGO)
  {
    if (CanSeePlayer(creatureGO))
    {
      creatureGO.rune = new('!');
    }
    else creatureGO.rune = creatureGO.entity.rune;

  }

}

// stand in place until sees player
// for now attack state will just change creature rune
public enum AIState { idle, attack }
