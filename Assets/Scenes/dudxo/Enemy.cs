using UnityEngine;
using System.Collections;
using TMPro;


public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyDataTable;

    public int id;
    public string enemyName;
    public int moveDelay;
    public int damage;
    public int attackDelay;
    public int hp;
    public Sprite monsterIcon;

    public HPDisplay hpDisplay;

    private SpriteRenderer spriteRenderer;
    private int currDelay = 0;
    private int currSpeed = 0;
    public int stepsize = 3;
    private int moveFrame = 5;
    private GameObject lastSpace;

    private bool isMove = true;
    private bool isAttack = false;

    private float knockBackDist = 0.1f;
    public GameObject hitEffectPrefab;
    public Transform hitPosition;

    public float[] damageWeight = new float[] { 0.5f, 0.75f, 1f };
    private int currWeightIndex = 0;


    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastSpace = GameObject.Find("can1");
    }

    public void Init(int eid)
    {
        this.id = eid;
        //Debug.Log(this.id);

        EnemyInfo info = enemyDataTable.GetEnemyById(this.id);

        this.enemyName = info.enemyName;
        this.moveDelay = info.moveDelay;
        this.damage = info.damage;
        this.attackDelay = info.attackDelay;
        this.hp = info.hp;

        if(spriteRenderer != null && info.monsterIcon != null)
        {
            spriteRenderer.sprite = info.monsterIcon;
        }

        //Debug.Log($"[몬스터 생성] ID: {id} / 이름: {enemyName} / HP: {hp} / 공격력: {damage}");
        hpDisplay.UpdateHP(hp);

    }
    public void Start()
    {
        EnemyManager.Instance.activeEnemy.Add(this);
    }


    void FixedUpdate()
    {
        if (isMove)
            Move();

        if (isAttack)
            Attack();

    }

    void Move()
    {
        currSpeed++;
        if (currSpeed >= moveDelay)
        {
            //transform.Translate(Vector2.left * stepsize);
            currWeightIndex++;
            currSpeed = 0;
        }
        if (currSpeed >= moveDelay - moveFrame && lastSpace.transform.position.x + 0.01f <= transform.position.x)
        {
            transform.Translate(Vector2.left * stepsize / moveFrame);
        }
        if (lastSpace.transform.position.x + 0.01f >= transform.position.x)
        {
            isAttack = true;
            isMove = false;
        }
    }

    void Attack()
    {
        currDelay++;
        if (currDelay >= attackDelay)
        {
            Player.Instance.TakeDamage(damage);
            currDelay = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.RoundToInt(damage * damageWeight[currWeightIndex]);
        this.hp -= finalDamage;
        if(this.hp <= 0)
        {
            Destroy(gameObject);
        }
        hpDisplay.UpdateHP(hp);

        StartCoroutine(TakeDamageMove(finalDamage));
    }

    IEnumerator TakeDamageMove(int damage)
    {
        transform.Translate(Vector2.right * knockBackDist * damage);
        spriteRenderer.color = new Color(1.0f, 0.4f, 0.4f, 1.0f);
        GameObject effect = Instantiate(hitEffectPrefab, hitPosition.position, Quaternion.identity);
        Destroy(effect, 0.2f);
        yield return new WaitForSeconds(0.2f);
        transform.Translate(Vector2.left * knockBackDist * damage);
        spriteRenderer.color = Color.white;
    }

    private void OnDestroy()
    {
        EnemyManager.Instance.activeEnemy.Remove(this);
    }
}
