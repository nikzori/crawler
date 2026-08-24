public class CreatureAttribute 
{
  public int BaseValue { get; set; }
  protected Dictionary<object, int> Modifiers { get; init; }
  public int Value { get => BaseValue + GetTotalModifier(); } 
  
  public int GetTotalModifier()
  {
    int result = 0;
    foreach(KeyValuePair<object, int> kvp in Modifiers)
        result += kvp.Value;
    return result;
  }
  public bool AddModifier(object source, int value)
  {
    if (Modifiers.ContainsKey(source))
        return false;
    else return Modifiers.TryAdd(source, value);
  }
  public bool RemoveModifier(object source, int value)
  {
      return Modifiers.Remove(source);
  }

  public CreatureAttribute (int baseValue)
  {
      BaseValue = baseValue;
      Modifiers = new();
  } 
}
