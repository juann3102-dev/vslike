using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Stage")]
public class StageDataSC : ScriptableObject
{
    [Header("Manual Stages")]
    public List<StageInfo> StageList = new List<StageInfo>();

    [Header("Automatic Stages")]
    [Range(1, 3)] public int MaxAutoSpawnCount = 3;
    [Min(0f)] public float HpGrowthPerStage = 0.08f;
    [Min(0f)] public float DamageGrowthPerStage = 0.04f;

    public bool TryGetManualStage(int stageNumber, out StageInfo stageInfo)
    {
        foreach (StageInfo stage in StageList)
        {
            if (stage != null && stage.StageNumber == stageNumber)
            {
                stageInfo = stage.CreateCopy();
                return true;
            }
        }

        stageInfo = null;
        return false;
    }

    public StageInfo CreateAutomaticStage(int stageNumber, int maxEnemyId)
    {
        StageInfo lastManualStage = null;
        foreach (StageInfo stage in StageList)
        {
            if (stage == null)
            {
                continue;
            }

            if (lastManualStage == null || stage.StageNumber > lastManualStage.StageNumber)
            {
                lastManualStage = stage;
            }
        }

        int lastManualNumber = lastManualStage != null ? lastManualStage.StageNumber : 0;
        int automaticStep = Mathf.Max(1, stageNumber - lastManualNumber);
        float baseHpMultiplier = lastManualStage != null ? lastManualStage.EnemyHpMultiplier : 1f;
        float baseDamageMultiplier = lastManualStage != null ? lastManualStage.EnemyDamageMultiplier : 1f;

        return new StageInfo
        {
            StageNumber = stageNumber,
            MaxEnemyId = Mathf.Max(0, maxEnemyId),
            SpawnCount = UnityEngine.Random.Range(1, Mathf.Clamp(MaxAutoSpawnCount, 1, 3) + 1),
            EnemyHpMultiplier = baseHpMultiplier + HpGrowthPerStage * automaticStep,
            EnemyDamageMultiplier = baseDamageMultiplier + DamageGrowthPerStage * automaticStep
        };
    }
}

[Serializable]
public class StageInfo
{
    [Min(1)] public int StageNumber = 1;
    [Min(0)] public int MaxEnemyId;
    [Range(1, 3)] public int SpawnCount = 1;
    [Min(0.01f)] public float EnemyHpMultiplier = 1f;
    [Min(0.01f)] public float EnemyDamageMultiplier = 1f;

    public StageInfo CreateCopy()
    {
        return new StageInfo
        {
            StageNumber = StageNumber,
            MaxEnemyId = MaxEnemyId,
            SpawnCount = SpawnCount,
            EnemyHpMultiplier = EnemyHpMultiplier,
            EnemyDamageMultiplier = EnemyDamageMultiplier
        };
    }
}
