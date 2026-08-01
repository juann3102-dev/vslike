using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public PlayerData playerData;
    public int hp;
    public int autoDamage;
    public int autoDelay;
    public Bullet[] defaultBullets;

    public HPDisplay hpDisplay;

    void Awake()
    {
        Application.targetFrameRate = 120;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        hp = playerData.playerInfo.hp;
        autoDamage = playerData.playerInfo.autoDamage;
        autoDelay = playerData.playerInfo.autoDelay;
        defaultBullets = playerData.playerInfo.defaultBullets;
        hpDisplay.UpdateHP(hp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        hpDisplay.UpdateHP(hp);
    }
}
