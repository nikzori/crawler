public class AI
{
    static Player player = Game.player;
    public static Random rng = new();
    static Path Pathfinder = new();
    Queue<Vector2Int>? currentPath;
    public static void Act(Creature creature, int aut)
    {
        creature.aut += aut;

        switch (creature.state)
        {
            case AIState.idle:
                if (CanSeePlayer(creature))
                {
                    creature.lastPlayerPosition = player.pos;
                    creature.state = AIState.attack;
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
                        Vector2Int randomPos;
                        int cntr = 0;
                        while (true)
                        {
                            x = rng.Next(-1, 2);
                            y = rng.Next(-1, 2);
                            randomPos = creature.pos + new Vector2Int(x, y);
                            if (creature.Move(randomPos) || cntr > 5)
                                break;
                            else cntr++;
                        }
                    }
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
                    IReadOnlyCollection<Vector2Int> newPath;
                    if (Pathfinder.Calculate(creature.pos, creature.lastPlayerPosition, Game.dungeon.GetCurrentFloor().GetObstacles(), out newPath))
                    {
                        creature.currentPath = newPath as Queue<Vector2Int>;
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
        int x = Math.Abs(creature.pos.X - player.pos.Y);
        int y = Math.Abs(creature.pos.Y - player.pos.Y);
        // since our FOV is square shaped (because of equidistant movement), 
        // we don't need to calculate the distance with a square root
        if (x > 10 || y > 10)
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
