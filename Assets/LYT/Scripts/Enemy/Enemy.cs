using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    //[SerializeField] private EnemyData EnemyData;
    private int randomRange;
    private EnemyInfo myData;
    private float currentHp;
    public event Action OnDie;
    DeckManager DeckManager;
    public int bulletid;
    public float bulletdamage;

    public void Initialize(EnemyInfo info)
    {
        myData = info;
        DeckManager = GetComponent<DeckManager>();
        DeckManager.Initialize(myData.Bullet_Setting);
        currentHp = myData.Enemy_HP;
        //Debug.Log("나의 체력은 : " + myData.Enemy_HP);
    }


    public void Update()
    {
        if (GameManager.Instance.PenaltyCh == false)
        {
            StartCoroutine(EnemyOnAttack());
            enabled = false;
        }
    }
    private IEnumerator EnemyOnAttack()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (GameManager.Instance.EndGameCh == true)
            {
                break;
            }
            if (DeckManager.CylinderList.Count == 0)
            {
                DeckManager.ReloadBullet();
                Debug.Log("적 재장전 중");
                yield return new WaitForSeconds(1.25f);
            }

            //Debug.Log
            //    ("공격! 탕! \n 실린더 : " + DeckManager.CylinderList.Count + " 덱 : " + DeckManager.DeckList.Count + " 사용된 : " + DeckManager.UsedList.Count); ;
            EnemyAttack();
            DeckManager.ShotBullet();
            yield return new WaitForSeconds(myData.Enemy_Delay);
        }
        //Debug.Log("게임이 종료되었습니다.");

    }

    public void EnemyAttack()
    {
        bulletid = DeckManager.CylinderList[0];
        BulletDataSC.BulletInfo info = DataManager.Instance.GetBulletInfo(bulletid);
        bulletdamage = info.BulletDamage;


        PlayerSC.Instance.Playertakedamage(bulletdamage);


    }

    public void Enemytakedamage(float attackdamage)
    {
        currentHp -= attackdamage;
        Debug.Log("으앙 (현재 체력 : " + currentHp + " ) " );
        if(currentHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager.Instance.EnemyDie();
        OnDie?.Invoke();
        Destroy(gameObject);
    }

    //1. 플레이어SC 참조
    //2. 공격 처리 && 자동 공격
    //3. 피격 매서드
    //4. 사망 처리
}
