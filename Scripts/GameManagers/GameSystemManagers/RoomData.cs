using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileRow
{
    public int [] col;
}

[System.Serializable]
public class RoomData
{
    public int id;
    public string name;
    public float tileSize;
    public int gridWidth;
    public int gridHeight;
    public TileRow[] tileGrid;
}

[System.Serializable]
public class RoomDataList
{
    public List<RoomData> rooms;
}
