using System;
using System.Collections.Generic;
using UnityEngine;

public enum BulletRarity
{
    Common,
    Rare,
    Epic
}

[CreateAssetMenu(menuName = "Data/Bullet")]
public class BulletDataSC : ScriptableObject
{
    public List<BulletInfo> BulletList = new List<BulletInfo>();

    [Serializable]
    public class BulletInfo
    {
        public int id;
        public string BulletName;
        public float BulletDamage;
        public BulletRarity Rarity;
    }
}
