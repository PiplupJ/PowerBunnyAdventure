using UnityEngine;
using System.Collections.Generic;


public class GridContext
{
    public int width;
    public int height;
    public Vector2 center => new( (width+1)/2, (height+1)/2);

    public bool InBounds(Vector2 point)
    {
        return point.x > 0 && point.x < width && point.y > 0 && point.y < height;
    }
}

public interface ISpawnPattern
{
    List<Vector2> GetCells(GridContext grid);
}

public readonly struct PatternEntry
{
    public readonly ISpawnPattern pattern;
    public readonly int tier;
    public readonly float weight;

    public PatternEntry(ISpawnPattern pattern, int tier, float weight)
    {
        this.pattern = pattern;
        this.tier = tier;
        this.weight = weight;
    }
}