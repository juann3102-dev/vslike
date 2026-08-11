using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Data References")]
    [SerializeField] private BulletDataSC bulletdata;

    private readonly Dictionary<int, BulletDataSC.BulletInfo> bulletInfoDict =
        new Dictionary<int, BulletDataSC.BulletInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitData();
    }

    private void InitData()
    {
        bulletInfoDict.Clear();

        if (bulletdata == null || bulletdata.BulletList == null)
        {
            Debug.LogError("BulletData가 할당되지 않았습니다.", this);
            return;
        }

        foreach (BulletDataSC.BulletInfo info in bulletdata.BulletList)
        {
            if (info == null)
            {
                continue;
            }

            if (!bulletInfoDict.TryAdd(info.id, info))
            {
                Debug.LogError($"중복된 총알 ID입니다. ID={info.id}", bulletdata);
            }
        }
    }

    public bool TryGetBulletInfo(int id, out BulletDataSC.BulletInfo info)
    {
        if (bulletInfoDict.TryGetValue(id, out info))
        {
            return true;
        }

        Debug.LogWarning($"등록되지 않은 총알 ID입니다. ID={id}", this);
        return false;
    }

    public List<BulletDataSC.BulletInfo> GetRewardChoices(int requestedCount)
    {
        List<BulletDataSC.BulletInfo> available =
            new List<BulletDataSC.BulletInfo>(bulletInfoDict.Values);
        List<BulletDataSC.BulletInfo> choices =
            new List<BulletDataSC.BulletInfo>();

        int choiceCount = Mathf.Max(0, requestedCount);
        while (choices.Count < choiceCount)
        {
            BulletRarity rarity = DrawAvailableRarity(available);
            List<BulletDataSC.BulletInfo> rarityPool = available.FindAll(
                bullet => bullet.Rarity == rarity);

            if (rarityPool.Count == 0)
            {
                break;
            }

            BulletDataSC.BulletInfo selected =
                rarityPool[Random.Range(0, rarityPool.Count)];
            choices.Add(selected);
        }

        return choices;
    }

    private static BulletRarity DrawAvailableRarity(
        List<BulletDataSC.BulletInfo> available)
    {
        bool hasCommon = available.Exists(bullet => bullet.Rarity == BulletRarity.Common);
        bool hasRare = available.Exists(bullet => bullet.Rarity == BulletRarity.Rare);
        bool hasEpic = available.Exists(bullet => bullet.Rarity == BulletRarity.Epic);

        float commonWeight = hasCommon ? 70f : 0f;
        float rareWeight = hasRare ? 25f : 0f;
        float epicWeight = hasEpic ? 5f : 0f;
        float totalWeight = commonWeight + rareWeight + epicWeight;
        float roll = Random.Range(0f, totalWeight);

        if (roll < commonWeight)
        {
            return BulletRarity.Common;
        }

        roll -= commonWeight;
        if (roll < rareWeight)
        {
            return BulletRarity.Rare;
        }

        return BulletRarity.Epic;
    }
}
