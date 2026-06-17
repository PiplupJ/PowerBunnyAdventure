using UnityEngine;
using System.Collections.Generic;

public class ClusterPattern : ISpawnPattern
{
    public List<Vector2> GetCells(GridContext grid)
    {
        Vector2 c = grid.center;

        List<Vector2> points = new List<Vector2>(9);

        
        for(int dx = -1; dx <=1; dx++){
            for(int dy = -1; dy <= 1; dy++){
                var p = new Vector2 (c.x + dx, c.y + dy);
                points.Add(p);
            }
        }

        return points;
    }
}
