using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerSC : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerDataSC PlayerData; // ScriptableObject 원본 데이터
    [SerializeField] private Text BulletText;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color darkColor;
    private float darkenFactor = 0.5f;
    private Audios Audios; 
    private DeckManager DeckManager;
    private Enemy enemy;
    private PlayerHP PlayerHP;
    public int bulletid;
    public float attackdamage;
    public bool isInputBlocked;
    public bool isTakeDamage=false;
    public float penaltyWait = 1.25f;
    private float PlayerHp;


    public static PlayerSC Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        DeckManager = GetComponent<DeckManager>();
        Audios = GetComponent<Audios>();
        PlayerHP = GetComponent<PlayerHP>();
        DeckManager.Initialize(PlayerData.PlayerList[0].Bullet_Setting);
        PlayerHp = PlayerData.PlayerList[0].HP;
        originalColor = spriteRenderer.color;
        //if (attackCompo == null) attackCompo = GetComponent<Attack>();
    }

    private void Start()
    {
        darkColor = new Color(
            originalColor.r * darkenFactor,
            originalColor.g * darkenFactor,
            originalColor.b * darkenFactor,
            originalColor.a
        );
        penaltyWait = 1.25f;
        PlayerHP.UIInitialize(PlayerHp);

    }


public void Update()
    {
        if(isInputBlocked)
        {
            return;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        { 
            if(GameManager.Instance.PenaltyCh == true)
            {
                StartCoroutine(PenaltyRoutine());
                Debug.Log("패널티!" + isInputBlocked);
            }
            else if (GameManager.Instance.PenaltyCh == false)
            {
                Debug.Log("공격 시작!" + isInputBlocked);
                StartCoroutine(AttackOn());
                enabled = false;
            }
        }

    }

    private IEnumerator AttackOn()
    {
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
                Debug.Log("재장전 중");
                BulletText.text = $"Reloadig";
                yield return new WaitForSeconds(1.25f);
                BulletText.text = $"{DeckManager.CylinderList.Count} / 6";
            }

            Debug.Log
                ("공격! 탕! \n 실린더 : " + DeckManager.CylinderList.Count + " 덱 : " + DeckManager.DeckList.Count + " 사용된 : " + DeckManager.UsedList.Count); ;
            PlayerAttack();
            Audios.PlayShot();
            DeckManager.ShotBullet();
            BulletText.text = $"{DeckManager.CylinderList.Count} / 6";
            yield return new WaitForSeconds(PlayerData.PlayerList[0].Shot_Delay);
        }
        Debug.Log("게임이 종료되었습니다.");
    }



    private IEnumerator PenaltyRoutine()
    {
        isInputBlocked = true; // 입력 차단 시작

        // (선택 사항) 패널티 연출 - UI 쿨타임 표시, 경고 사운드, 캐릭터 붉은색 변경 등

        yield return new WaitForSeconds(penaltyWait); // 1.25초 대기

        isInputBlocked = false; // 입력 차단 해제
    }



    public void changetarget()
    {
        if (GameManager.Instance.StageManager.EnemyList.Count == 0)
        {
            Debug.Log("더 이상 적이 없습니다.");
        }
        else
        {
            enemy = GameManager.Instance.StageManager.EnemyList[0].GetComponent<Enemy>();
        }
    }

    public void PlayerAttack()
    {
        bulletid = DeckManager.CylinderList[0];
        BulletDataSC.BulletInfo info = DataManager.Instance.GetBulletInfo(bulletid);
        attackdamage = info.BulletDamage;

        //Debug.Log("공격 데미지는 : " + attackdamage);
        if (enemy != null)
        {
            enemy.Enemytakedamage(attackdamage);
        }
        else
        {
            Debug.LogWarning("공격할 적(enemy)이 지정되지 않았습니다!");
        }

    }

    public void Playertakedamage(float attackdamage)
    {
        PlayerHp -= attackdamage;
        PlayerHP.UpdateHPBar(PlayerHp);
        if (isTakeDamage == false)
        {
            StartCoroutine(Darken());
        }
    
        Debug.Log("플레이어 피격! (현재 체력 : " + PlayerHp + " ) ");
        if (PlayerHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator Darken()
    {
        //isTakeDamage = true;
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = darkColor;
            yield return new WaitForSeconds(0.0625f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.0625f);
        }
        //isTakeDamage = false;
    }

    public void Die()
    {
        GameManager.Instance.PlayerDie();
    }

    //타겟 지정
    //공격 매서드
    //사망 처리
    //피격 처리

}