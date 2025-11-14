public static class AI
{
    static Player player = Game.player;
    public static Random rng = new();
    static Path Pathfinder = new();
    public static void Act(Creature creature, int aut)
    {
        creature.aut += aut;

        switch (creature.state)
        {
            case AIState.idle:
                Vector2Int nextPos;
                if (!creature.currentPath.TryDequeue(out nextPos)) //current path queue is empty
                {
                    while (true) // pick a random point nearby to go to
                    {
                        Vector2Int nPos = new(rng.Next(creature.pos.X - 5, creature.pos.X + 5), rng.Next(creature.pos.Y - 5, creature.pos.Y + 5));
                        if (Game.currentMap.cells.ContainsKey(nPos))
                        {
                            if (Game.currentMap.cells[nPos].isWalkable && Pathfinder.Calculate(creature.pos, nPos, Game.currentMap.GetObstacles(), ref creature.currentPath))
                            {
                                string path = "";
                                foreach (Vector2Int vctr in creature.currentPath)
                                {
                                    path += vctr.ToString() + "; ";
                                }
                                if (creature.currentPath.TryDequeue(out nextPos))
                                    break;
                            }
                        }
                    }
                }
                if (creature.aut >= creature.MoveSpeed)
                {
                    creature.MoveTo(nextPos);
                    creature.aut -= creature.MoveSpeed;
                }
                break;

            case AIState.attack:
                if (CanSeePlayer(creature))
                {
                    creature.lastPlayerPosition = player.pos;
                    // weigh all action options, see if action can be performed (check cooldowns and such)
                    if (CanReachAttack(creature, Game.player))
                        UI.Log(creature.name + " attacks " + Game.player.name);
                    else
                    {
                        // approach the player
                    }
                }
                else goto case AIState.pursuit;
                break;

            case AIState.pursuit:
                if (CanSeePlayer(creature))
                    goto case AIState.attack;
                else if (creature.currentPath?.Last() == creature.lastPlayerPosition) // player stayed still, no need to update
                    creature.MoveTo(creature.currentPath.Dequeue());
                else
                {
                    if (Pathfinder.Calculate(creature.pos, creature.lastPlayerPosition, Game.dungeon.GetCurrentFloor().GetObstacles(), ref creature.currentPath))
                    {
                        creature.MoveTo(creature.currentPath.Dequeue());
                    }
                }
                break;
        }

        // things to add:
        // sleep timer
    }

    public static bool CanSeePlayer(Creature creature)
    {
        //check whether the Player is in vision range in the first place
        Vector2Int distance = player.pos - creature.pos;
        // since our FOV is square shaped (because of equidistant movement), 
        // we don't need to calculate the distance with a square root
        if (Math.Abs(distance.X) > 10 || Math.Abs(distance.Y) > 10)
            return false;

        else return Dungeon.CanSeeTile(creature.pos, player.pos);
    }

    public static bool CanReachAttack(Creature host, Creature target)
    {
        if (Math.Abs(host.pos.X - target.pos.X) <= 1 && Math.Abs(host.pos.Y - target.pos.Y) <= 1)
            return true;
        else return false;
    }
}

public enum AIState { idle, attack, pursuit, sleep }
