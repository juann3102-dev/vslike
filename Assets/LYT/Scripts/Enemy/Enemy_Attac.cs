using UnityEngine;

public class Enemy_Attac : MonoBehaviour
{
    [SerializeField] private EnemyData EnemyData;
    [SerializeField] private float attackInterval = 0f;
    private float attackTimer = 0f;

    void start()
    {
        EnemyInfo enemy = EnemyData.EnemyList[0];
    }
    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer > attackInterval)
        {
            attack();
            attackTimer -= attackInterval;
        }
    }

    void attack()
    {
        Debug.Log("공격!");
    }
}
