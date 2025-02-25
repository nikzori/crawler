public static class AI
{
  static GameObject playerGO = Game.playerGO;
  public static Dictionary<Creature, AIState> creatures = new();

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
          if (creature.gameObject.Move(x, y) || cntr > 3)
            break;
          else cntr++;
          
        }        
      }
    }
  }

  public static void Act(int aut)
  {
    foreach(Creature creature in creatures.Keys)
    {
      creature.aut += aut;

      switch(creatures[creature])
      {
        case AIState.idle:
          if (CanSeePlayer(creature.gameObject))
          {
            creatures[creature] = AIState.attack;
            goto case AIState.attack; // no idea if this works
          }
          else
          {
            // go to sleep if timer/counter requires sleep
            // obviously, gotta add extra timer for that
            // otherwise pick random unoccupied point in sight, A* to it, idle in place for 30-50 aut, go to next place
          }
        break;

        case AIState.attack:
          if (CanSeePlayer(creature.gameObject))
          {
            // update player position
            // weigh all action options, see if action can be performed (check cooldowns and such)
            if (CanReachAttack(creature.gameObject, Game.playerGO)) //this should be internal function for either entity or abilities
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
          // go to the last place where the player was seen 
          // walk around?
          if (CanSeePlayer(creature.gameObject))
            goto case AIState.attack;
        break;

        case AIState.sleep:
          // deduct aut from sleep timer
          // if counter at 0, 
          // goto case AIState.idle;
        break;
      }
      if (CanSeePlayer(creature.gameObject))
      {
        creatures[creature] = AIState.attack;
        // check if any abilities can reach player, use them if not on cooldown
        // if creature has ranged weapon/role, stay and use that
        // if melee, A* to the player
      }
      else if(creatures[creature] == AIState.pursuit)
      {
        
      }
      else 
      {
        // go to sleep if conditions are met
        // otherwise pick walkable tile in sight, A* to it (this should probably account for pack tactics and movement)
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
public enum AIState { idle, attack, pursuit, sleep }
