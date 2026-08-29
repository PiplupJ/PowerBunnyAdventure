using UnityEngine;
using System.Collections.Generic;

//敵生成パターン
//最上の一列
public class EdgeLinePattern : ISpawnPattern
{
    public List<Vector2> GetCells(GridContext grid)
    {
        List<Vector2> points = new List<Vector2>();

        for(int i = 2; i < grid.width - 2; i++)
        {
            points.Add(new Vector2(i , 1));
        }
        
        return points;
    }
}
