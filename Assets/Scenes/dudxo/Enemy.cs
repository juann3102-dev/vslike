using UnityEngine;

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

    private SpriteRenderer spriteRenderer;
    private int currDelay = 0;
    public int stepsize = 3;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int eid)
    {
        this.id = eid;
        Debug.Log(this.id);

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

        Debug.Log($"[몬스터 생성] ID: {id} / 이름: {enemyName} / HP: {hp} / 공격력: {damage}");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currDelay++;
        if (currDelay >= delay)
        {
            transform.Translate(Vector2.left * stepsize);
            currDelay = 0;
        }
    }
}
