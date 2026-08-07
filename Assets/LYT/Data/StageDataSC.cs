using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

[CreateAssetMenu(menuName = "Data/Stage")]

public class StageDataSC : ScriptableObject
{

    public List<StageInfo> StageList;

}
[System.Serializable]
public class StageInfo
{
    public int Spawn_Enemy_ID;
    public int Spawn_Enemy_num;
    public float status_magnification;

}