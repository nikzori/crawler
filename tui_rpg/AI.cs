public static class AI
{
  static GameObject playerGO = Game.playerGO;
  static Dictionary<GameObject, (AIState state, int autStored)> creatureDict = new();

  // to prevent interruptions of actions and allow creatures to perform actions across several Player turns,
  // we need a variable to store available time
  // the AIState redundant for now, but will probably be useful for pursuit, wandering around and other fluff
  public static void Act(GameObject creatureGO, int aut)
  {
    // check whether the creature can see the player to do something besides idling
    if (CanSeePlayer(creatureGO))
    {
      //
      int newAut = creatureDict[creatureGO].autStored + aut;
      creatureDict[creatureGO] = (AIState.attack, newAut);
      // check whether the player is in range of any attack
      if (CanReachAttack(creatureGO, playerGO)) // and has enough aut to act !!!
      {
        // attack
      }
      else if (CanReachPoint(creatureGO, playerGO.pos))
      {
        // attempt movement
        // since this is being run every turn it's fine to go to the player position
      }
      else
      {
        // can't get to player, can't attack player => idle
        creatureDict[creatureGO] = (AIState.idle, 0);
      }
    }
    else
    {
      // can't see player, go idle
      // resetting time to 0 to avoid stacking it off screen
      creatureDict[creatureGO] = (AIState.idle, 0);
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
      (int x, int y)[] points = Map.GetLine(creatureGO.pos, playerGO.pos);
      for (int i = 0; i < points.Length; i++)
      {
        if (Game.map.cells[points[i].x, points[i].y].isTransparent == false)
          return false;
      }
      return true;
    }
  }

  public static bool CanReachPoint(GameObject creatureGO, (int x, int y) pos)
  {
    return false;
  }

  // Check if creature's attack can reach the target
  public static bool CanReachAttack(GameObject host, GameObject target)
  {
    return false;
  }
}

// stand in place until sees player
// for now attack state will just change creature rune
public enum AIState { idle, attack, pursuit }
