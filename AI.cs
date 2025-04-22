public static class AI
{
  static Player player = Game.player;
  public static Dictionary<Creature, AIState> creatures = new();
  public static Random rng = new();
  // simple test for creature and map code I have so far
  public static void TestAct(int aut)
  {
    foreach (Creature creature in creatures.Keys)
    {
      creature.aut += aut;
      if (creature.aut >= 10)
      {
        Random rng = new Random();
        int x, y;
        int cntr = 0;
        while (true)
        {
          x = rng.Next(-1, 2);
          y = rng.Next(-1, 2);
          if (creature.Move(x, y) || cntr > 3)
            break;
          else cntr++;

        }
      }
    }
  }

  public static void Act(int aut)
  {
    foreach (Creature creature in creatures.Keys)
    {
      creature.aut += aut;

      switch (creatures[creature])
      {
        case AIState.idle:
          if (CanSeePlayer(creature))
          {
            creatures[creature] = AIState.attack;
            goto case AIState.attack; // no idea if this works
          }
          else
          {
            if (rng.Next(0, 100) <= 1) // this should probably be a separate timer
            {
              creature.sleepTimer = 200 + aut;
              goto case AIState.sleep;
            }
            // do nothing or take a step
          }
          break;

        case AIState.attack:
          if (CanSeePlayer(creature))
          {
            // update player position
            // weigh all action options, see if action can be performed (check cooldowns and such)
            if (CanReachAttack(creature, Game.player))
            {
              // use ability
            }
            else
            {
              // navigate closer to player
            }
          }
          else goto case AIState.pursuit;
          break;

        case AIState.pursuit:
          if (CanSeePlayer(creature))
            goto case AIState.attack;
          // go to the last place where the player was seen 
          // walk around?

          break;

        case AIState.sleep:
          creature.sleepTimer -= aut;
          if (creature.sleepTimer <= 0)
            goto case AIState.idle;
          break;
      }

      // things to add:
      // sleep timer
      // enemy position tracker
      // path to tracked enemy
      // action interface
      // 
    }
  }

  public static bool CanSeePlayer(Creature creature)
  {
    //check whether the Player is in vision range in the first place
    int x = Math.Abs(creature.pos.x - player.pos.x);
    int y = Math.Abs(creature.pos.y - player.pos.y);
    // since our FOV is square shaped (because of equidistant movement), 
    // we don't need to calculate the distance with a square root
    if (x > 10 || y > 10)
      return false;

    else return Dungeon.CanSeeTile(creature.pos, player.pos);
  }

  public static bool CanReachPoint(Creature creature, (int x, int y) target) // should add a ref variable also
  {
    // an attempt at implementing A*; I have no idea what I'm doing
    /*
      starting from the tile that the creature occupies, we go through each cell around it;
      for each cell, if a cell is walkable and is not already in the list of valid cells, calculate:
      `g` - movement cost to get to this cell (in aut)
      `h` - estimated movement cost (in aut) from this cell to target cell; just draw a bresenham's line 
      the cell with the smallest value of `g + h` is considered the best cell to take, and goes into a list of valid cells
      repeat until the target cell is reached
      

    */

    // to find a path, we need a list of tiles that the creature would have to cross
    List<(int x, int y)> path = new();

    return false;
  }


  public static bool CanReachAttack(Creature host, Creature target)
  {
    if (Math.Abs(host.pos.x - target.pos.x) <= 1 && Math.Abs(host.pos.y - target.pos.x) <= 1)
      return true;
    else return false;
  }
}

// stand in place until sees player
// for now attack state will just change creature rune
public enum AIState { idle, attack, pursuit, sleep }
