using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct TestData
{
    public int id;
    public string Name;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "TestDB", menuName = "Scriptable Objects/TestDB")]
public class TestDB : ScriptableObject
{
    public List<TestData> testdatas = new List<TestData>();
}
