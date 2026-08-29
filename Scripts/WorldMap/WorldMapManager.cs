using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum WorldMoveAngle
{
    Left, Right
}

public class WorldMapManager : MonoBehaviour
{
    [SerializeField] private List<WorldNode> worldList;

    private Dictionary<int, WorldNode> worldDict = new Dictionary<int, WorldNode>();

    private int currentIndex;

    public WorldNode CurrentNode => worldList[currentIndex];

    public TransitionController transition;

    private void Awake()
    {
        Cache_Init();
        currentIndex = 0;
        EnableNodeDescription();
    }

    private void Cache_Init()
    {
        for(int i = 0; i < worldList.Count; i++)
        {
            var w = worldList[i];

            if(worldDict.ContainsKey(i))
            {
               Debug.LogWarning($"重複Worldが存在します！");
            }
            else
            {
               worldDict[i] = w;
            }
        }
    }

    public void LoadWorldScene()
    {
        SceneManager.LoadScene(CurrentNode.sceneName);
    }

    public void OnStageTransition()
    {
        transition.StartTransition(LoadWorldScene);
    }

    public void EnterCurrentWorld()
    {
        if(!CurrentNode.canEnter) { return; }
        OnStageTransition();
    }

    public bool HasWorldToMove(WorldMoveAngle angle)
    {
        return  GetNextNode(angle) != null;
    }

    public Vector3 GetDestination(WorldMoveAngle angle)
    {
        return GetNextNode(angle)?.WorldPos ?? Vector3.zero;
    }

    public void UpdateWorldIndex(WorldMoveAngle angle)
    {
        switch(angle)
        {
            case WorldMoveAngle.Left :
                currentIndex -= 1;
                break;
            case WorldMoveAngle.Right :
                currentIndex += 1;
                break;
        }
    }

    private WorldNode GetNextNode(WorldMoveAngle angle)
    {
        return angle == WorldMoveAngle.Right ? CurrentNode.right : CurrentNode.left;
    }

    public void EnableNodeDescription()
    {
        CurrentNode.description.SetActive(true);
    }

    public void DisableNodeDescription()
    {
        CurrentNode.description.SetActive(false);
    }
}
