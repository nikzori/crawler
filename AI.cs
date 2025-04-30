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
                        // do nothing or take a step
                        if (rng.Next(0, 101) < 5)
                            return;
                        else
                        {
                            int x, y;
                            int cntr = 0;
                            while (true)
                            {
                                x = rng.Next(-1, 2);
                                y = rng.Next(-1, 2);
                                if (creature.Move(x, y) || cntr > 5)
                                    break;
                                else cntr++;
                            }
                        }
                    }
                    break;

                case AIState.attack:
                    if (CanSeePlayer(creature))
                    {
                        // update player position
                        // weigh all action options, see if action can be performed (check cooldowns and such)
                        if (CanReachAttack(creature, Game.player))
                        {
                            UI.Log(creature.name + " attacks " + Game.player.name);
                        }
                        else
                        {
                            creature.MoveTo(Navigate(creature.pos, Game.player.pos)[0]);
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
            }

            // things to add:
            // sleep timer
            // enemy position tracker
            // path to tracked enemy
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

    public static (int x, int y)[] Navigate((int x, int y) start, (int x, int y) target) // should maybe add a ref variable also
    {
        // an attempt at implementing A*; I have no idea what I'm doing
        // I can probably reduce this to a single tile, also
        /*
          starting from the tile that the creature occupies, we go through each cell around it;
          for each cell, if a cell is walkable and is not already in the list of valid cells, calculate:
          `g` - movement cost to get to this cell (in aut)
          `h` - estimated movement cost (in aut) from this cell to target cell; just draw a bresenham's line 
          the cell with the smallest value of `g + h` is considered the best cell to take, and goes into a list of valid cells
          repeat until the target cell is reached
        */

        List<(int x, int y)> path = new(); // final list of all tiles
        Cell[,] map = Game.currentMap.cells; // shortcut to current map
        List<(int x, int y)> adjacents = new();
        while (true)
        {
            for (int xt = -1; xt < 2; xt++)
            {
                for (int yt = -1; yt < 2; yt++) // cycle through tiles around the creature
                {
                    int dx = start.x + xt;
                    int dy = start.y + yt;

                    if (dx <= 0 || dy <= 0 || dx >= map.GetLength(0) || dy >= map.GetLength(1))
                        continue;

                    if (map[dx, dy].IsWalkable() && !path.Contains((dx, dy)))
                        adjacents.Add((dx, dy));
                }
            }
            if (adjacents.Count == 0)
                break;
            (int x, int y) bestTile = adjacents[0];
            foreach ((int x, int y) position in adjacents)
            {

                int g = path.Count;
                int hPos = Dungeon.GetLine(position, target).Length;
                int hCur = Dungeon.GetLine(bestTile, target).Length;
                if ((g + hPos) < (g + hCur))
                    bestTile = position;
            }
            path.Add(bestTile);
        }
        return path.ToArray();
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
