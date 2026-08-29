using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StatusData
{
    public StatusType type;
    public bool canRefresh;
    public bool hasDOT;
    public bool isMovable;
    [Header("DOT")]
    public float tickInterval;
    public float dotPercent;
    [Header("Modifiers")]
    public float moveModifier;
    public float attackModifier;
}

[CreateAssetMenu(fileName = "StatusConfig", menuName = "Scriptable Objects/StatusConfig")]
public class StatusConfig : ScriptableObject
{
    public List<StatusData> statuses;

    private Dictionary<StatusType, StatusData> _cache;

    public StatusData Get(StatusType type)
    {
        if(_cache == null)
        {
            _cache = new Dictionary<StatusType, StatusData>();

            foreach(var s in statuses)
            {
                _cache[s.type] = s;
            }
        }
        
        if(_cache.TryGetValue(type, out var data))
        {
            return data;
        }
        
        return null;
    }
}
