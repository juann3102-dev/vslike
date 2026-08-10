using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    //[SerializeField] private EnemyData EnemyData;
    [SerializeField] private Text BulletText;
    private EnemyHealth health;
    private Audios Audios;
    private int randomRange;
    private EnemyInfo myData;
    private float currentHp;
    public event Action OnDie;
    DeckManager DeckManager;
    public int bulletid;
    public float bulletdamage;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color darkColor;
    private float darkenFactor = 0.5f;


    void Awake()
    {
        Audios = GetComponent<Audios>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<EnemyHealth>();
        originalColor = spriteRenderer.color;
        darkColor = new Color(
            originalColor.r * darkenFactor,
            originalColor.g * darkenFactor,
            originalColor.b * darkenFactor,
            originalColor.a
        );
    }

    public void Initialize(EnemyInfo info)
    {
        myData = info;
        DeckManager = GetComponent<DeckManager>();
        DeckManager.Initialize(myData.Bullet_Setting);
        currentHp = myData.Enemy_HP;
        health.UIInitialize(currentHp);
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
                Audios.PlayReload();
                Debug.Log("적 재장전 중");
                BulletText.text = $"Reloadig...";
                yield return new WaitForSeconds(1.25f);
                BulletText.text = $"{DeckManager.CylinderList.Count} / 6";
            }

            //Debug.Log
            //    ("공격! 탕! \n 실린더 : " + DeckManager.CylinderList.Count + " 덱 : " + DeckManager.DeckList.Count + " 사용된 : " + DeckManager.UsedList.Count); ;
            EnemyAttack();
            Audios.PlayShot();
            DeckManager.ShotBullet();
            BulletText.text = $"{DeckManager.CylinderList.Count} / 6";
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
        health.UpdateHPBar(currentHp);
        StartCoroutine(Darken());
        Debug.Log("으앙 (현재 체력 : " + currentHp + " ) " );
        if(currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator Darken()
    {
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = darkColor;
            yield return new WaitForSeconds(0.0625f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.0625f);
        }
    }
        //isT

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
