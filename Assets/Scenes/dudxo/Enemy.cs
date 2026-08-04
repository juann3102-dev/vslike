using UnityEngine;
using System.Collections;
using TMPro;


public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyDataTable;

    public int id;
    public string enemyName;
    public int speed;
    public int damage;
    public int delay;
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

    private float knockBackDist = 0.2f;
    public GameObject hitEffectPrefab;
    public Transform hitPosition;


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
        this.speed = info.speed;
        this.damage = info.damage;
        this.delay = info.delay;
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
        if (currSpeed >= speed)
        {
            //transform.Translate(Vector2.left * stepsize);
            currSpeed = 0;
        }
        if (currSpeed >= speed - moveFrame && lastSpace.transform.position.x + 0.01f <= transform.position.x)
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
        if (currDelay >= delay)
        {
            Player.Instance.TakeDamage(damage);
            currDelay = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        this.hp -= damage;
        if(this.hp <= 0)
        {
            Destroy(gameObject);
        }
        hpDisplay.UpdateHP(hp);

        StartCoroutine(TakeDamageMove());
    }

    IEnumerator TakeDamageMove()
    {
        transform.Translate(Vector2.right * knockBackDist);
        spriteRenderer.color = new Color(1.0f, 0.4f, 0.4f, 1.0f);
        GameObject effect = Instantiate(hitEffectPrefab, hitPosition.position, Quaternion.identity);
        Destroy(effect, 0.2f);
        yield return new WaitForSeconds(0.2f);
        transform.Translate(Vector2.left * knockBackDist);
        spriteRenderer.color = Color.white;
    }

    private void OnDestroy()
    {
        EnemyManager.Instance.activeEnemy.Remove(this);
    }
}
