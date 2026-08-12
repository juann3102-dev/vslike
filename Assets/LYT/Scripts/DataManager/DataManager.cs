using System.Collections.Generic;
using UnityEngine;

public enum BulletRewardAction
{
    AddBullet,
    RemoveBullet
}

public sealed class BulletRewardChoice
{
    public BulletRewardAction Action { get; }
    public BulletDataSC.BulletInfo Bullet { get; }

    public BulletRewardChoice(
        BulletRewardAction action,
        BulletDataSC.BulletInfo bullet = null)
    {
        Action = action;
        Bullet = bullet;
    }
}

public sealed class OwnedBulletChoice
{
    public BulletDataSC.BulletInfo Bullet { get; }
    public int Count { get; }

    public OwnedBulletChoice(BulletDataSC.BulletInfo bullet, int count)
    {
        Bullet = bullet;
        Count = count;
    }
}

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

    public List<BulletRewardChoice> GetRewardChoices(int requestedCount)
    {
        List<BulletDataSC.BulletInfo> available =
            new List<BulletDataSC.BulletInfo>(bulletInfoDict.Values);
        List<BulletRewardChoice> choices = new List<BulletRewardChoice>();

        int choiceCount = Mathf.Max(0, requestedCount);
        while (choices.Count < choiceCount)
        {
            choices.Add(DrawRewardChoice(available));
        }

        return choices;
    }

    public List<OwnedBulletChoice> GetOwnedBulletChoices(
        Dictionary<int, int> bulletCounts)
    {
        List<OwnedBulletChoice> choices = new List<OwnedBulletChoice>();
        if (bulletCounts == null)
        {
            return choices;
        }

        List<int> ids = new List<int>(bulletCounts.Keys);
        ids.Sort();

        foreach (int id in ids)
        {
            if (bulletCounts[id] <= 0 ||
                !TryGetBulletInfo(id, out BulletDataSC.BulletInfo info))
            {
                continue;
            }

            choices.Add(new OwnedBulletChoice(info, bulletCounts[id]));
        }

        return choices;
    }

    private static BulletRewardChoice DrawRewardChoice(
        List<BulletDataSC.BulletInfo> available)
    {
        bool hasCommon = available.Exists(bullet => bullet.Rarity == BulletRarity.Common);
        bool hasRare = available.Exists(bullet => bullet.Rarity == BulletRarity.Rare);
        bool hasEpic = available.Exists(bullet => bullet.Rarity == BulletRarity.Epic);

        float commonWeight = hasCommon ? 70f : 0f;
        float rareWeight = hasRare ? 20f : 0f;
        float epicWeight = hasEpic ? 5f : 0f;
        const float removalWeight = 5f;
        float totalWeight = commonWeight + rareWeight + epicWeight + removalWeight;
        float roll = Random.Range(0f, totalWeight);

        if (roll < commonWeight)
        {
            return CreateBulletChoice(available, BulletRarity.Common);
        }

        roll -= commonWeight;
        if (roll < rareWeight)
        {
            return CreateBulletChoice(available, BulletRarity.Rare);
        }

        roll -= rareWeight;
        if (roll < epicWeight)
        {
            return CreateBulletChoice(available, BulletRarity.Epic);
        }

        return new BulletRewardChoice(BulletRewardAction.RemoveBullet);
    }

    private static BulletRewardChoice CreateBulletChoice(
        List<BulletDataSC.BulletInfo> available,
        BulletRarity rarity)
    {
        List<BulletDataSC.BulletInfo> rarityPool = available.FindAll(
            bullet => bullet.Rarity == rarity);
        BulletDataSC.BulletInfo selected =
            rarityPool[Random.Range(0, rarityPool.Count)];
        return new BulletRewardChoice(BulletRewardAction.AddBullet, selected);
    }
}
