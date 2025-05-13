public class AI
{
    static Player player = Game.player;
    public static Random rng = new();
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
                    creature.lastPlayerPosition = Game.player.pos;
                    // weigh all action options, see if action can be performed (check cooldowns and such)
                    if (CanReachAttack(creature, Game.player))
                        UI.Log(creature.name + " attacks " + Game.player.name);
                    else
                    {
                        List<Node> path = Pathfinding.GetPath(creature.pos, Game.player.pos);
                        if (path.Count > 0)
                            creature.MoveTo(path[0].position);
                        else
                        {
                            UI.Log(creature.name + " cannot find path to player");
                            UI.Log("Path node count: " + path.Count);
                        }
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
