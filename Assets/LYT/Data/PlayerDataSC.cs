using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.ShaderGraph.Internal;

[CreateAssetMenu(menuName = "Data/Player")]

public class PlayerDataSC : ScriptableObject
{

    public List<PlayerInfo> PlayerList;

    [System.Serializable]
    public class PlayerInfo
    {
        public float HP;
        public int revolver_Cylinder_Gauge;
        public float Shot_Delay;
        public float attack_magnification;
        public int[] Bullet_Setting = new int[15];
    }

}
