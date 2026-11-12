public static class PlayerController
{
    public static Creature Player = Game.Player;
    public static void CellInteract(Vector2Int pos)
    {
        if (Game.CurrentMap.TryBump(Player, pos))
        {
            Game.Update(5);
        }
    }
    public static void Attack(Creature target)
    {
        Player.Attack(target);
    }
}
