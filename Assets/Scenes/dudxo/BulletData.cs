using UnityEngine;


[System.Serializable]
public struct BulletInfo
{
    public int id;
    public int attack;
    public int targetCount;
    //public int distance;
}

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    public BulletInfo[] bulletList;
    public BulletInfo GetBulletById(int id)
    {
        foreach (var info in bulletList)
        {
            if (info.id == id)
            {
                return info;
            }
        }

        Debug.Log("총알을 찾을 수 없습니다.");
        return bulletList[0];
    }
}
