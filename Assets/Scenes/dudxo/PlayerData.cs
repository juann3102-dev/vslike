using UnityEngine;

[System.Serializable]
public struct PlayerInfo
{
    public int hp;
    public int autoDamage;
    public int autoDelay;
    public int[] defaultBulletsById;
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public PlayerInfo playerInfo;
}
