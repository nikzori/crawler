public static class JSONParse
{

  //de-serializes JSON files
  static Resources LoadResources()
  {
    return new Resources();
  }
}

public class Resources
{
  public List<Item> items;

  public Resources()
  {
    items = new List<Item>();
  }
}
