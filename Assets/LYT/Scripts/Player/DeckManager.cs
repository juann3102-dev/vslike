using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    //[SerializeField] private PlayerDataSC PlayerData;
    private static int cardtemp;

    public List<int> DeckList = new List<int>();
    public List<int> CylinderList = new List<int>();
    public List<int> UsedList = new List<int>();

    /*
    void Awake() //플레이어 데이터에서 총알 구성 받아오기
    {
        DeckList = new List<int>((int[])PlayerData.PlayerList[0].Bullet_Setting);
        DeckShuffle();
    }*/

    public void Initialize(int[] bulletSetting)
    {
        DeckList = new List<int>(bulletSetting);
        DeckShuffle();
        ReloadBullet();
    }


    private void DeckShuffle()
    {
        for (int i = DeckList.Count-1; i >0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i+1);
            cardtemp = DeckList[i];
            DeckList[i] = DeckList[randomIndex];
            DeckList[randomIndex] = cardtemp;
        }
        /*
        for(int i = 0; i <  DeckList.Count; i++)
        {
            Debug.Log("섞은 후 : " + DeckList[i]);
        }*/

    }

    //덱에서 실린더로 총알 이동
    public void ReloadBullet()
    {
        // 실린더가 5발 미만일 때만 총알을 계속 채움
        while (CylinderList.Count < 6)
        {
            if (DeckList.Count == 0)
            {
                UsedBullet();
            }

            CylinderList.Add(DeckList[0]);
            DeckList.RemoveAt(0);
        }

        // 5발 완충이 완료되면 공격 호출
        /*Debug.Log
            ("덱 : "+DeckList.Count+"\n실린더 : "+CylinderList.Count+"\n사용된 : "+UsedList.Count);*/

    }

    //실린더에서 사용된 총알로 이동
    public void ShotBullet()
    {
        if(CylinderList.Count == 0)
        {
            Debug.Log("실린더에 총알이 없습니다.");
        }
        UsedList.Add(CylinderList[0]);
        CylinderList.RemoveAt(0);

        /*Debug.Log
            ("덱 : " + DeckList.Count + "\n실린더 : " + CylinderList.Count + "\n사용된 : " + UsedList.Count);*/
    }

    //사용된 총알에서 덱으로 이동
    private void UsedBullet()
    {
        while (UsedList.Count > 0)
        {
            DeckList.Add(UsedList[0]);
            UsedList.RemoveAt(0);
        }

        DeckShuffle();

        Debug.Log
            ("덱 : " + DeckList.Count + "\n실린더 : " + CylinderList.Count + "\n사용된 : " + UsedList.Count);
    }

}