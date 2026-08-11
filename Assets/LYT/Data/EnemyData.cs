using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    public List<EnemyInfo> EnemyList = new List<EnemyInfo>();

    public int MaxEnemyId
    {
        get
        {
            int maxId = -1;
            foreach (EnemyInfo info in EnemyList)
            {
                if (info != null && info.Enemy_ID > maxId)
                {
                    maxId = info.Enemy_ID;
                }
            }

            return maxId;
        }
    }

    public bool TryGetEnemyInfo(int id, out EnemyInfo result)
    {
        foreach (EnemyInfo info in EnemyList)
        {
            if (info != null && info.Enemy_ID == id)
            {
                result = info;
                return true;
            }
        }

        result = null;
        return false;
    }
}

[Serializable]
public class EnemyInfo
{
    public string name;
    public int Enemy_ID;
    public float Enemy_HP;
    public float Enemy_Delay;
    public float Enemy_Start_Shot;
    public int[] Bullet_Setting = new int[5];
}
