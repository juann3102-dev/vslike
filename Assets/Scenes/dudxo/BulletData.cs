using UnityEngine;

[System.Serializable]
public struct BulletInfo
{
    public int id;
    public int attack;
    public int targets;
    //public int distance;
}

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    public BulletInfo[] bulletList;
}
