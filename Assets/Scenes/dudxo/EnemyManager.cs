using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public GameObject lastSpace;

    public static EnemyManager Instance;
    public List<Enemy> activeEnemy = new List<Enemy>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 테스트용: 숫자키 1, 2, 3을 누르면 해당 ID의 몬스터 생성
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnEnemy(0); // 101번 ID 몬스터 생성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnEnemy(1); // 102번 ID 몬스터 생성
        }
    }

    public void SpawnEnemy(int monsterId)
    {
        float randomY = Random.Range(-3f, 3f);
        Vector2 spawnPosition = new Vector2(spawnPoint.position.x, randomY);

        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Enemy enemy = newEnemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Init(monsterId);
        }
    }

    public Enemy GetNearest(Vector3 playerPos)
    {
        if (activeEnemy.Count == 0) return null;

        Enemy nearest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < activeEnemy.Count; i++)
        {
            if (activeEnemy[i] == null) continue;

            float distSqr = (activeEnemy[i].transform.position - playerPos).sqrMagnitude;
            if (distSqr < minDistance)
            {
                nearest = activeEnemy[i];
                minDistance = distSqr;
            }
        }
        return nearest;
    }

    public List<Enemy> GetNearestEnemies(Vector3 origin, int count)
    {
        // LINQ를 활용해 거리 순 정렬 후 count 개만큼 추출
        return activeEnemy
            .Where(enemy => enemy != null && enemy.gameObject.activeInHierarchy) 
            .OrderBy(enemy => Vector3.Distance(origin, enemy.transform.position))  
            .Take(count)
            .ToList();
    }
}