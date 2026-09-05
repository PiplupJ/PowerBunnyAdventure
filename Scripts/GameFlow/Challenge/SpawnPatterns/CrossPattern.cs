using UnityEngine;
using System.Collections.Generic;

//敵生成パターン
//クロスで生成
public class CrossPattern : ISpawnPattern
{
    public List<Vector2> GetCells(GridContext grid)
    {
        Vector2 c = grid.center;

        List<Vector2> points = new List<Vector2>();

        int limit = (int)(c.x - 2);

        for(int i = 1; i < limit; i++)
        {
            points.Add(new Vector2(c.x + i , c.y));
            points.Add(new Vector2(c.x - i , c.y));
            points.Add(new Vector2(c.x, c.y + i));
            points.Add(new Vector2(c.x, c.y - i));
        }

        points.Add(c);
        
        return points;
    }
}
