using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Bullet")]

public class BulletDataSC : ScriptableObject
{

    public List<BulletInfo> BulletList;
    [System.Serializable]
    public class BulletInfo
    {
        public int id;
        public string BulletName;
        public float BulletDamage;
    }

}