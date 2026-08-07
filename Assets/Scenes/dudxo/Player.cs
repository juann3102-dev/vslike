using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public PlayerData playerData;
    private int hp;
    private int autoDamage;
    private int autoDelay;
    public List<int> defaultBullets;

    private int currDelay;

    private float knockBackDist = 0.2f;
    public GameObject hitEffectPrefab;
    public Transform hitPosition;

    public HPDisplay hpDisplay;

    public GameObject sword;
    public AudioSource audioSource;
    public AudioClip swordClip;

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
        defaultBullets = new List<int>(playerData.playerInfo.defaultBulletsById);
        hpDisplay.UpdateHP(hp);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            BulletManager.Instance.FireBullet();
        }
    }

    void FixedUpdate()
    {
        currDelay++;
        if(currDelay >= autoDelay)
        {
            AutoAttack();
            currDelay = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        hpDisplay.UpdateHP(hp);

        StartCoroutine(TakeDamageMove());

        if(hp <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    IEnumerator TakeDamageMove()
    {
        transform.Translate(Vector2.left * knockBackDist);
        GameObject effect = Instantiate(hitEffectPrefab, hitPosition.position, Quaternion.identity);
        Destroy(effect, 0.2f);
        yield return new WaitForSeconds(0.2f);
        transform.Translate(Vector2.right * knockBackDist);
    }

    void AutoAttack()
    {
        Enemy nearest = EnemyManager.Instance.GetNearest(this.transform.position);
        if (nearest != null)
        {
            nearest.TakeDamage(autoDamage);
            audioSource.PlayOneShot(swordClip);
            Instantiate(sword);
            //Debug.Log($"{nearest.enemyName} 공격");
        }
        //Debug.Log("함수 호출");
    }
}
