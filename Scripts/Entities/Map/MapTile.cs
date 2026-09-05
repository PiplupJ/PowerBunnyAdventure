using UnityEngine;


public class MapTile : PoolableObject
{   
    //継承用。基本タイルは何もしないが、特殊タイルはPlayerに効果を与える
    public virtual void OnInteract(Player player)
    {
        
    }
}
