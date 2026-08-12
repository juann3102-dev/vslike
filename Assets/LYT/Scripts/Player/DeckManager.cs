using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public const int CylinderCapacity = 6;

    public List<int> DeckList = new List<int>();
    public List<int> CylinderList = new List<int>();
    public List<int> UsedList = new List<int>();

    public void Initialize(int[] bulletSetting)
    {
        DeckList = bulletSetting != null
            ? new List<int>(bulletSetting)
            : new List<int>();
        CylinderList = new List<int>();
        UsedList = new List<int>();

        ShuffleDeck();
        ReloadBullet();
    }

    public void AddBullet(int bulletId)
    {
        DeckList.Add(bulletId);
    }

    public void PrepareForNextStage()
    {
        DeckList.AddRange(CylinderList);
        DeckList.AddRange(UsedList);
        CylinderList.Clear();
        UsedList.Clear();

        ShuffleDeck();
        ReloadBullet();
    }

    public void ReloadBullet()
    {
        while (CylinderList.Count < CylinderCapacity)
        {
            if (DeckList.Count == 0)
            {
                RecycleUsedBullets();
            }

            if (DeckList.Count == 0)
            {
                break;
            }

            CylinderList.Add(DeckList[0]);
            DeckList.RemoveAt(0);
        }
    }

    public bool ShotBullet()
    {
        if (CylinderList.Count == 0)
        {
            Debug.LogWarning("실린더에 총알이 없습니다.", this);
            return false;
        }

        UsedList.Add(CylinderList[0]);
        CylinderList.RemoveAt(0);
        return true;
    }

    public int GetTotalBulletCount()
    {
        return DeckList.Count + CylinderList.Count + UsedList.Count;
    }

    public Dictionary<int, int> GetBulletCounts()
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();
        AddCounts(DeckList, counts);
        AddCounts(CylinderList, counts);
        AddCounts(UsedList, counts);
        return counts;
    }

    public bool RemoveBullet(int bulletId)
    {
        if (DeckList.Remove(bulletId))
        {
            return true;
        }

        if (UsedList.Remove(bulletId))
        {
            return true;
        }

        return CylinderList.Remove(bulletId);
    }

    private void RecycleUsedBullets()
    {
        if (UsedList.Count == 0)
        {
            return;
        }

        DeckList.AddRange(UsedList);
        UsedList.Clear();
        ShuffleDeck();
    }

    private static void AddCounts(
        List<int> source,
        Dictionary<int, int> counts)
    {
        foreach (int bulletId in source)
        {
            if (counts.TryGetValue(bulletId, out int count))
            {
                counts[bulletId] = count + 1;
            }
            else
            {
                counts.Add(bulletId, 1);
            }
        }
    }

    private void ShuffleDeck()
    {
        for (int i = DeckList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (DeckList[i], DeckList[randomIndex]) =
                (DeckList[randomIndex], DeckList[i]);
        }
    }
}
