using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

//TODO : Interface名の修正。collision以外の要素があるのでSystemへ
public interface IMapCollision
{
    bool MapWallHitCheck(Vector3 pos, float rad);
    bool MapWalkableCheck(Vector3 pos, float rad);
    void HitTileCheck(Vector3 pos, float rad);
}
public interface IMapSystem : IMapCollision
{
    Vector3 GridToWorldSpace(int gridX, int gridY);
}

public class TileType
{
    public const int FLOOR = 0;
    public const int START = 1;
    public const int GOAL = 2;
    public const int WALL = 3;
    public const int WATER = 4;

    private static readonly HashSet<int> _walkable = new HashSet<int>
        { FLOOR, START, GOAL};

    public static bool IsWalkable(int tile) => _walkable.Contains(tile);
}
public class MapManager : IMapSystem
{
    public int mapWidth;
    public int mapHeight;
    public float mapTileSize;

    private int [,] mapGrid;

    private Dictionary<int, RoomData> roomDict; //部屋情報

    public Vector3 startPos; //開始時プレイヤー配置座標
    public Vector3 goalPos;

    public event Action touchedFinish;

    private PoolableObject currentRoom; //現在の部屋
    private PoolableObject startGate; //入口ゲート
    private PoolableObject goalGate;  //出口ゲート
    private PoolableObject startPortal; //スタートポータル
    private PoolableObject goalPortal; //ゴールポータル

    private List<Vector3> _floorPositions = new List<Vector3>();
    
    private static readonly Dictionary<StageType, int> portalIDs = new Dictionary<StageType, int>
    {
        { StageType.Normal, IDRegistry.PORTAL_NORMAL },
        { StageType.Boss,   IDRegistry.PORTAL_BOSS   },
        { StageType.Bonus,  IDRegistry.PORTAL_BONUS  }
    };

    public MapManager()
    {
        roomDict = new Dictionary<int, RoomData>();
        LoadRoomDatas();
    }
    //JSONから部屋情報を読み込む
    private void LoadRoomDatas()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/RoomDatas");

        if(jsonText != null)
        {
            RoomDataList list = JsonUtility.FromJson<RoomDataList>(jsonText.text);

            foreach(RoomData data in list.rooms){
                roomDict.Add(data.id, data);
            }
        }
    }
    //部屋情報初期化
    public void MapInit(int RoomID, StageType type)
    {
        MapClear();

        RoomData roomData = roomDict[RoomID];

        mapWidth = roomData.gridWidth;
        mapHeight = roomData.gridHeight;
        mapTileSize = roomData.tileSize;
        mapGrid = new int[mapWidth, mapHeight];

        Debug.Log($"MapInit — RoomID:{RoomID} startPos before:{startPos}");

        for(int z = 0; z < mapHeight; z++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                int tileData = roomData.tileGrid[z].col[x];
                mapGrid[x,z] = tileData;

                switch(tileData)
                {
                    case TileType.START :
                        startPos = GridToWorldSpace(x, z);
                        startPortal = ObjectPool.Instance.GetObject<PoolableObject>(portalIDs[type]);
                        startPortal.transform.position = startPos;
                        break;
                    case TileType.GOAL :
                        goalPos = GridToWorldSpace(x, z);
                        goalGate = ObjectPool.Instance.GetObject<PoolableObject>(IDRegistry.GATE_GOAL);
                        goalGate.transform.position = goalPos;
                        mapGrid[x,z] = TileType.WALL;
                        break;
                    default :
                        break;
                }
            }
        }

        Debug.Log($"MapInit — startPos after:{startPos}");
        currentRoom = ObjectPool.Instance.GetObject<PoolableObject>(RoomID);
    }
    //現在のマップを削除
    private void MapClear()
    {
        if(currentRoom == null)
        {
            return;
        }
        currentRoom.ReturnToPool();
        startGate?.ReturnToPool();
        goalGate?.ReturnToPool();
        startPortal?.ReturnToPool();
        goalPortal?.ReturnToPool();

        _floorPositions = null;
    }
    //グリッドをワールド座標へ
    public Vector3 GridToWorldSpace(int gridX, int gridY)
    {
        float WorldPosX = gridX * mapTileSize;
        float WorldPosZ = -gridY * mapTileSize;

        return new Vector3(WorldPosX, 0, WorldPosZ);
    }
    //ワールド座標をグリッドへ(X軸)
    public int WorldSpaceToGridIndexX(float worldPosX)
    {
        return (int)((worldPosX/mapTileSize)+0.5f);
    }
    //ワールド座標をグリッドへ(Z軸)
    public int WorldSpaceToGridIndexZ(float worldPosZ)
    {
        return (int)((-worldPosZ/mapTileSize)+0.5f);
    }
    //ステージ開始時実行 部屋の入口を閉じる
    public void BlockStart()
    {
        startGate = ObjectPool.Instance.GetObject<PoolableObject>(IDRegistry.GATE_START);
        startGate.transform.position = startPos;

        int x = WorldSpaceToGridIndexX(startPos.x);
        int z = WorldSpaceToGridIndexZ(startPos.z);

        mapGrid[x,z] = TileType.WALL;
        startPortal?.ReturnToPool();
        Debug.Log("BlockStart Done");
    }
    //ステージクリア時実行　部屋の出口を開ける
    public void OpenGate(StageType nextStage)
    {
        int x = WorldSpaceToGridIndexX(goalGate.transform.position.x);
        int z = WorldSpaceToGridIndexZ(goalGate.transform.position.z);

        mapGrid[x,z] = TileType.GOAL;

        goalGate.ReturnToPool();
        goalPortal = ObjectPool.Instance.GetObject<PoolableObject>(portalIDs[nextStage]);
        goalPortal.transform.position = goalPos;
        Debug.Log("Open Gate Done");
    }
    //壁に衝突確認
    public bool MapWallHitCheck(Vector3 pos, float rad)
        => CheckTiles(pos, rad, tile => tile == TileType.WALL);
    //歩けるタイルかを確認
    public bool MapWalkableCheck(Vector3 pos, float rad)
        => CheckTiles(pos, rad, tile => TileType.IsWalkable(tile));
    //立っているタイルの種類を確認
    public void HitTileCheck(Vector3 pos, float rad)
    {
        if(CheckTiles(pos, rad, tile=>tile==TileType.GOAL)){
            touchedFinish?.Invoke();
        }
    }
    //座標にあるタイルがどの状態かを確認
    private bool CheckTiles(Vector3 pos, float rad, Func<int, bool> condition)
    {
        int left = WorldSpaceToGridIndexX(pos.x - rad);
        int top = WorldSpaceToGridIndexZ(pos.z + rad);
        int right = WorldSpaceToGridIndexX(pos.x + rad);
        int bottom = WorldSpaceToGridIndexZ(pos.z - rad);

        for(int x = left; x <= right; x++){
            for(int z = top; z<=bottom; z++){
                //配列範囲外なら衝突判定
                if(OutOfBound(x,z)){return true;}
                if(condition(mapGrid[x,z])){return true;}
            }
        }
        return false;
    }
    //マップ範囲外確認
    private bool OutOfBound(int x, int y)
    {
        return x < 0 || x >= mapWidth || y < 0 || y >= mapHeight;
    }
    //部屋の中央座標を返却
    public Vector3 GetCenterPosition()
    {
        return GridToWorldSpace(mapWidth/2, mapHeight/2);
    }
    //部屋でランダム座標を返却
    public List<Vector3> GetRandomFloorPositions(int count)
    {
        //必要な時のみ、リストを生成
        if (_floorPositions == null)
        {
            _floorPositions = new List<Vector3>();
            for (int z = 0; z < mapHeight; z++)
                for (int x = 0; x < mapWidth; x++)
                    if (mapGrid[x, z] == TileType.FLOOR)
                        _floorPositions.Add(GridToWorldSpace(x, z));
        }

        List<Vector3> available = new List<Vector3>(_floorPositions);
        List<Vector3> result    = new List<Vector3>();
        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }

        return result;
    }
}
