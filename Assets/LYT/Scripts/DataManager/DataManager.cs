using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    [Header("Data References")]
    [SerializeField] private BulletDataSC bulletdata;
    private Dictionary<int, BulletDataSC.BulletInfo> bulletInfoDict;

    private void Awake()
    {
        // 싱글톤 중복 방지 및 유지 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitData()
    {
        bulletInfoDict = new Dictionary<int, BulletDataSC.BulletInfo>();

        if (bulletdata == null || bulletdata.BulletList == null)
        {
            Debug.LogError("BulletData가 할당되지 않았습니다!");
            return;
        }

        // 딕셔너리에 캐싱하여 조회 성능 최적화 (O(1))
        foreach (var info in bulletdata.BulletList)
        {
            if (!bulletInfoDict.ContainsKey(info.id))
            {
                bulletInfoDict.Add(info.id, info);
            }
        }
    }

    /// <summary>
    /// ID로 BulletInfo 정보 가져오기
    /// </summary>
    public BulletDataSC.BulletInfo GetBulletInfo(int id)
    {
        if (bulletInfoDict.TryGetValue(id, out var info))
        {
            return info;
        }

        Debug.LogWarning($"[DataManager] ID {id}에 해당하는 BulletInfo가 없습니다.");
        return null;
    }
}
