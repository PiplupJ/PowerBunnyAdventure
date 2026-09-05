using UnityEngine;
using System.Collections.Generic;

//敵配置パターン
//角で生成
public class CornersPattern : ISpawnPattern
{
    public List<Vector2> GetCells(GridContext grid)
    {

        List<Vector2> points = new List<Vector2>(16);

        Vector2 leftTop = new Vector2(1,1);
        Vector2 rightTop = new Vector2(grid.width - 2, 1);
        Vector2 leftBottom = new Vector2(1, grid.height - 2);
        Vector2 rightBottom = new Vector2(grid.width - 2, grid.height - 2);

        for(int i = 0; i<=1; i++)
        {
            points.Add(new Vector2(leftTop.x + i, leftTop.y));
            points.Add(new Vector2(leftTop.x + i, leftTop.y + 1));
            
            points.Add(new Vector2(rightTop.x - i, rightTop.y));
            points.Add(new Vector2(rightTop.x - i, rightTop.y + 1));

            points.Add(new Vector2(leftBottom.x + i, leftBottom.y));
            points.Add(new Vector2(leftBottom.x + i, leftBottom.y - 1));

            points.Add(new Vector2(rightBottom.x - i, rightBottom.y));
            points.Add(new Vector2(rightBottom.x - i, rightBottom.y - 1));

        }
       
        return points;
    }
}
