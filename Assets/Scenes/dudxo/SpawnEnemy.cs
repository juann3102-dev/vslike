using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public GameObject lastSpace;

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
}