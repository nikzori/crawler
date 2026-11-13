public class TotalArmorAttribute : CreatureAttribute
{
    private readonly Creature owner;

    public TotalArmorAttribute(Creature owner, int baseValue = 0) : base(baseValue)
    {
        this.owner = owner;
    }

    public int EquippedArmorValue
    {
        get
        {
            if (owner?.Equipment == null) return 0;
            int sum = 0;
            foreach (KeyValuePair<EquipmentSlot, Equippable> kvp in owner.Equipment)
            {
                if (kvp.Value is Armor armor)
                {
                    sum += armor.ArmorValue;
                }
            }
            return sum;
        }
    }

    public override int Value => BaseValue + EquippedArmorValue + GetTotalModifier();

    public static implicit operator int(TotalArmorAttribute attr) => attr?.Value ?? 0;

    public override string ToString() => Value.ToString();
}
