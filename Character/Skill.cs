public class Skill : CreatureAttribute
{
    public CreatureAttribute BaseAttribute { get; init; }

    public override int Value { get => BaseValue + (BaseAttribute.Value / 4) + GetTotalModifier(); }
    public Skill(int baseValue, CreatureAttribute baseAttribute) : base(baseValue)
    {
      this.BaseValue = baseValue;
      this.BaseAttribute = baseAttribute;
    } 
}
