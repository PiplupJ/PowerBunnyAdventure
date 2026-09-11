using UnityEngine;
using System.Collections.Generic;

public interface IObjectPool
{
    void ReturnObject(int poolId, PoolableObject obj);
}
public class ObjectPool : MonoBehaviour, IObjectPool
{
    public static ObjectPool Instance;

    //オブジェクトプールが持っているオブジェクトDB
    //インスペクタでドラッグ＆ドロップすること

    [SerializeField] private PlayerDB    playerDB;
    [SerializeField] private EnemyDB     enemyDB;
    [SerializeField] private ShotDB      playerShotDB;
    [SerializeField] private ShotDB      enemyShotDB;
    [SerializeField] private MapDB       mapDB;
    [SerializeField] private EffectDB    effectDB;
    [SerializeField] private UIElementDB uiDB;
    [SerializeField] private ItemDB      itemDB;
    [SerializeField] private AbilityCardDB abilityDB;
    
    private PoolDataDB[] _allDBs;


    //検索速度のため、Cacheを作成
    private Dictionary<int, PoolableObject> objectCache = new Dictionary<int, PoolableObject>();
    //実際にオブジェクトを管理する。IDに相当するオブジェクトをQueueに保存
    private Dictionary<int, Queue<PoolableObject>> poolDict = new Dictionary<int, Queue<PoolableObject>>();

    //初期化：自分をインスタンス化、Cacheデータを作成
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _allDBs = new PoolDataDB[]
        {
            playerDB, enemyDB, playerShotDB, enemyShotDB, effectDB,
            uiDB, abilityDB, itemDB, mapDB
        };

        Cache_Init();
    }

    //Cacheを作成
    private void Cache_Init()
    {
        foreach(PoolDataDB db in _allDBs)
        {
            if(db == null) continue;
            foreach(var d in db.data)
            {
                if(objectCache.ContainsKey(d.id)){
                    Debug.LogWarning($"重複IDが存在します！ID:{d.id}in:{db.name}");
                }
                else{
                    objectCache[d.id] = d.prefab;
                }
            }
        }
    }

    //外部からオブジェクトを求める場合呼び出す
    public bool TryGetObject<T>(int id, out T obj) where T : PoolableObject
    {
        obj = GetObject<T>(id);
        return obj != null;
    }

    //PoolableObjectを継承しているクラスを返却
    //呼び出す時、TはPoolableObjectを継承したクラスを指定すること
    private T GetObject<T>(int id) where T : PoolableObject
    {
        //idのpoolがないなら作成
        if(!poolDict.ContainsKey(id))
        {
            poolDict.Add(id, new Queue<PoolableObject>());
        }

        //借りるオブジェクトがあれば出す
        if(poolDict[id].Count>0)
        {
            T pooledObj = poolDict[id].Dequeue() as T;
            if(pooledObj == null)
            {
                Debug.LogError($"[ObjectPool]{id}のプールに{typeof(T).Name}と一致しているオブジェクトがありません");
                return null;
            }
            pooledObj.gameObject.SetActive(true);
            pooledObj.isPooled = false;
            return pooledObj;
        }
        else{
            //ないなら生成
            if (objectCache.TryGetValue(id, out PoolableObject prefab))
	        {
                if(prefab == null)
                {
                    Debug.LogError($"[ObjectPool]{id}のプールに{typeof(T).Name}と一致しているオブジェクトがありません");
                    return null;
                }

                PoolableObject baseObj = Instantiate(prefab, transform);
                
                baseObj.poolId = id;
                baseObj.myPool = this;

                //返却のため、型を再定義
                T newObj = baseObj as T;
                if(newObj == null)
                {
                    Debug.LogError($"[ObjectPool]{id}のプールに{typeof(T).Name}と一致しているオブジェクトがありません");
                    return null;
                }

                newObj.isPooled = false;

	            return newObj;
	        }
        }
        //例外処理
        Debug.LogError($"[ObjectPool]{id}のプールに{typeof(T).Name}と一致しているオブジェクトがありません");
        return null;
    }

    //使用が終わったらpoolへ
    //PoolableObjectがReturnToPool()で実行
    public void ReturnObject(int id, PoolableObject obj)
    {
        if(obj == null) { return;}
        if(obj.isPooled == true){ 
            Debug.LogWarning($"[ObjectPool]{id}のオブジェクトが２重返却");
            return;
        }

        
        //待機キューが未実装だった
        if(!poolDict.TryGetValue(id, out Queue<PoolableObject> queue))
        {
            queue = new Queue<PoolableObject>();
            poolDict.Add(id, queue);
        }

        obj.gameObject.SetActive(false);
        obj.isPooled = true;
        queue.Enqueue(obj);
    }
}
