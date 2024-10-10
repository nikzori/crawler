public class AI
{
  // 
  public GameObject go;

  public AI(GameObject go)
  {
    this.go = go;
    if (go.entity == null)
      return; // idk if this works; on the other hand, this shouldn't even be evoked in a gObj without a Creature;

  }
}

// expand later
public enum AIState { idle, sleeping, attack, pursuit }
