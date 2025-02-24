/// <summary>
/// Static class for `.json` deserialization.
/// </summary>
public static class JSONParse
{
  
  
  public static void Init()
  {
    try
    {
      
    }
    catch (Exception err)
    {
      UI.Log("Error while parsing .json files: " + err.Message);
    }

  }
}

