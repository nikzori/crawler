public class Creature : IDamageable
{
    //const int BASE_MOVESPEED = 10;
    const int BASE_SIGHTRADIUS = 10;
    public _Faction Faction { get; set; }

    // attribute values and derivatives are to be finetuned
    public CreatureAttribute Strength { get; }
    public CreatureAttribute Dexterity { get; }
    public CreatureAttribute Intelligence { get; }

    public Skill Dodge;
    public Skill Shields;
    public Skill Armor;
    
    public Skill Unarmed;
    public Skill ShortBlades;
    public Skill LongBlades;
    public Skill Blunt;
    public Skill Polearms;
    
    public Skill Ranged;
    public Skill Thrown;

    public string Name;
    public int MaxHealth { get => 10 + (Strength.Value / 4); }
    public float Health { get; set; }
    public int SightRadius { get; set; }
    public Vector2Int Pos { get; set; }
    
    const float MIN_MOVE_TIME = 10;
    public float MovementTime { get => Math.Max(MIN_MOVE_TIME - (Dexterity.Value / 5), 2); }
    public float AttackTime { get; set; }

    public Inventory Inventory { get; }
    public Dictionary<EquipmentSlot, Equippable> Equipment = new();

    public Creature(string name, Vector2Int pos, int sightRadius = BASE_SIGHTRADIUS,
            float movementSpeed = 5f, float attackSpeed = 5f, _Faction faction = _Faction.Dungeon)
    {
        Strength = new(10);
        Dexterity = new(10);
        Intelligence = new(10);

        Dodge = new(10, Dexterity);
        Shields = new(10, Strength);
        Armor = new(10, Strength);

        Unarmed = new(10, Strength);
        ShortBlades = new(10, Dexterity);
        LongBlades = new(10, Dexterity);
        Blunt = new(10, Strength);
        Polearms = new(10, Strength);

        Ranged = new(10, Dexterity);
        Thrown = new(10, Strength);

        this.Name = name;
        this.Pos = pos;
        this.Health = MaxHealth;
        this.SightRadius = sightRadius;

        this.Faction = faction;
        this.Inventory = new();
    }

    // Shorthand for normalized vectors
    public bool Move(Vector2Int dir) { return Game.CurrentMap.TryBump(this, dir); }

    public bool MoveTo(Vector2Int pos)
    {
        return Game.CurrentMap.TryMoveCreature(this, pos);
    }

    public void Attack(Creature target)
    {
        Game.Log($"{Name} attacks {target.Name}!");
        target.ReceiveDamage(1f);
    }

    public void ReceiveDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            OnCreatureDeath();
    }

    void OnCreatureDeath() { 
        Game.Log($"{Name} died!");
        Game.CurrentMap.creatures.Remove(this); 
        Game.CurrentMap.RemoveCreature(this); 
    }


}

public interface IDamageable
{
    float Health { get; set; }
    void ReceiveDamage(float damage);
}

public enum _Faction { Dungeon, Player }
