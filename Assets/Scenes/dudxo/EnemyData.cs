using UnityEngine;

[System.Serializable]
public struct EnemyInfo
{
    public int id;
    public string enemyName;
    public int speed;
    public int damage;
    public int delay;
    public int hp;
    public Sprite monsterIcon;
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyInfo[] enemyList;
    public EnemyInfo GetEnemyById(int id)
    {
        foreach (var info in enemyList)
        {
            if (info.id == id)
            {
                return info;
            }
        }

        Debug.Log("몬스터를 찾을 수 없습니다");
        return enemyList[0];
    }
}
