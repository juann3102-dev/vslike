using UnityEngine;
using System.Collections.Generic;
//using TMPro;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletData bulletDataTable;

    public int id;
    public int attack;
    public int targetCount;

    //public TextMeshProUGUI bulletText;

    public void Init(int bid)
    {
        this.id = bid;
        //Debug.Log(this.id);

        BulletInfo info = bulletDataTable.GetBulletById(this.id);

        this.attack = info.attack;
        this.targetCount = info.targetCount;
        //bulletText.text = this.id.ToString();
    }

    void Start()
    {
        List<Enemy> targets = EnemyManager.Instance.GetNearestEnemies(Player.Instance.transform.position, targetCount);
        if (targets.Count <= 0) {
            Debug.Log("공격할 대상이 없습니다.");
            return;
        }

        foreach (Enemy enemy in targets)
        {
            enemy.TakeDamage(attack);
        }

        Destroy(gameObject);
    }

}
