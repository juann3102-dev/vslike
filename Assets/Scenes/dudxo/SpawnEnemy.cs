using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Enemy 프리팹
    [SerializeField] private Transform spawnPoint;   // 생성 위치

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
        // 1. Enemy 프리팹 생성
        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // 2. 생성된 Enemy 스크립트에 접근
        Enemy enemy = newEnemyObj.GetComponent<Enemy>();

        // 3. 원하는 ID 전달하여 스탯 및 데이터 로드
        if (enemy != null)
        {
            enemy.Init(monsterId);
        }
    }
}