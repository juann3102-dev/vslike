using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class StageManager : MonoBehaviour
{
    [SerializeField] private EnemyData EnemyData;
    [SerializeField] private PlayerSC PlayerSC;
    private int randomIndex;
    public int randomCount;
    public Transform[] SpawnPoint;
    public GameObject EnemyPrefab;
    public List<Enemy> EnemyList = new List<Enemy>();

    void Start()
    {
        //randomCount = UnityEngine.Random.Range(0, 2);
        randomCount = UnityEngine.Random.Range(0,2);
        for (int i = 0; i <= randomCount; i++)
        {
            Enemy_Spawn(i);
        }
        PlayerSC.changetarget();
    }

    void Enemy_Spawn(int i)
    {
        randomIndex = UnityEngine.Random.Range(0, EnemyData.EnemyList.Count);
        EnemyInfo EnemyIn = EnemyData.EnemyList[randomIndex];
        GameObject obj = Instantiate(EnemyPrefab, SpawnPoint[i].position, SpawnPoint[i].rotation);

        Enemy EnemySC = obj.GetComponent<Enemy>();
        EnemySC.Initialize(EnemyIn);
        EnemySC.OnDie += RemoveEnemy;
        EnemyList.Add(EnemySC);
        
    }

    public void RemoveEnemy()
    {
        EnemyList.RemoveAt(0);
        PlayerSC.changetarget();
    }
}

//, SpawnPoint[i]
