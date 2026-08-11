using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.ShaderGraph.Internal;

[CreateAssetMenu(menuName = "Data/Enemy")]

public class EnemyData : ScriptableObject
{

    public List<EnemyInfo> EnemyList;

}
[System.Serializable]
public class EnemyInfo
{
    public string name;
    public int Enemy_ID;
    public float Enemy_HP;
    public float Enemy_Delay;
    public float Enemy_Start_Shot;
    public int[] Bullet_Setting = new int[5];
}