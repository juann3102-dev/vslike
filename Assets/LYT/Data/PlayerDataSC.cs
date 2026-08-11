using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player")]
public class PlayerDataSC : ScriptableObject
{
    public List<PlayerInfo> PlayerList = new List<PlayerInfo>();

    [Serializable]
    public class PlayerInfo
    {
        public float HP;
        public int revolver_Cylinder_Gauge;
        public float Shot_Delay;
        public float attack_magnification;
        public int[] Bullet_Setting = new int[15];
    }
}
