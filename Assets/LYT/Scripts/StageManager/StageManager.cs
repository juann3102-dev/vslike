using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private EnemyData EnemyData;
    [SerializeField] private StageDataSC StageData;
    [SerializeField] private PlayerSC PlayerSC;

    public Transform[] SpawnPoint;
    public GameObject EnemyPrefab;
    public List<Enemy> EnemyList = new List<Enemy>();

    public int CurrentStageNumber { get; private set; }
    public StageInfo CurrentStageInfo { get; private set; }

    private GameManager gameManager;

    public void Initialize(GameManager owner)
    {
        gameManager = owner;
    }

    public bool StartStage(int stageNumber)
    {
        if (!ValidateReferences())
        {
            return false;
        }

        ClearRemainingEnemies();
        CurrentStageNumber = stageNumber;

        if (!StageData.TryGetManualStage(stageNumber, out StageInfo stageInfo))
        {
            stageInfo = StageData.CreateAutomaticStage(stageNumber, EnemyData.MaxEnemyId);
        }

        CurrentStageInfo = stageInfo;
        int spawnCount = Mathf.Clamp(
            stageInfo.SpawnCount,
            1,
            Mathf.Min(3, SpawnPoint.Length));
        int maxEnemyId = Mathf.Clamp(
            stageInfo.MaxEnemyId,
            0,
            EnemyData.MaxEnemyId);

        for (int i = 0; i < spawnCount; i++)
        {
            int enemyId = Random.Range(0, maxEnemyId + 1);
            if (!EnemyData.TryGetEnemyInfo(enemyId, out EnemyInfo enemyInfo))
            {
                Debug.LogError($"스테이지가 참조한 적 ID가 없습니다. ID={enemyId}", this);
                continue;
            }

            SpawnEnemy(enemyInfo, SpawnPoint[i]);
        }

        PlayerSC.changetarget();
        return EnemyList.Count > 0;
    }

    private void SpawnEnemy(EnemyInfo enemyInfo, Transform spawnPoint)
    {
        GameObject enemyObject = Instantiate(
            EnemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation);
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        if (enemy == null || !enemy.Initialize(
                enemyInfo,
                CurrentStageInfo.EnemyHpMultiplier,
                CurrentStageInfo.EnemyDamageMultiplier))
        {
            Debug.LogError("적 프리팹 초기화에 실패했습니다.", enemyObject);
            Destroy(enemyObject);
            return;
        }

        enemy.OnDie += RemoveEnemy;
        EnemyList.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.OnDie -= RemoveEnemy;
        }

        EnemyList.Remove(enemy);
        PlayerSC.changetarget();

        if (EnemyList.Count == 0)
        {
            gameManager.EnemyDie();
        }
    }

    private bool ValidateReferences()
    {
        if (EnemyData == null || StageData == null || PlayerSC == null ||
            EnemyPrefab == null || SpawnPoint == null || SpawnPoint.Length == 0)
        {
            Debug.LogError("StageManager 참조가 누락되었습니다.", this);
            return false;
        }

        if (EnemyData.EnemyList == null || EnemyData.EnemyList.Count == 0)
        {
            Debug.LogError("EnemyData가 비어 있습니다.", this);
            return false;
        }

        return true;
    }

    private void ClearRemainingEnemies()
    {
        foreach (Enemy enemy in EnemyList)
        {
            if (enemy != null)
            {
                enemy.OnDie -= RemoveEnemy;
                Destroy(enemy.gameObject);
            }
        }

        EnemyList.Clear();
    }
}
