using UnityEngine;

public class WorldNode : MonoBehaviour
{
    public bool canEnter;
    public string sceneName;
    public WorldNode right;
    public WorldNode left;
    public Vector3 WorldPos => transform.position;
    public GameObject description;
}
