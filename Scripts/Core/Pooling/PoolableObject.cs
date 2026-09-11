using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    //ObjectPool内で使うID
    public int poolId;
    //自分が戻る場所
    public IObjectPool myPool;
    public bool isPooled;
    //使用が終わったらObjectPoolに戻る
    public void ReturnToPool()
    {
        myPool.ReturnObject(this.poolId, this);
    }
}
