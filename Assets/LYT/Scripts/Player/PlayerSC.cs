using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(DeckManager), typeof(PlayerHP), typeof(Audios))]
public class PlayerSC : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private PlayerDataSC PlayerData;
    [SerializeField] private Text BulletText;
    [SerializeField] private float penaltyWait = 1.25f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color darkColor;
    private Audios audios;
    private DeckManager deckManager;
    private Enemy targetEnemy;
    private PlayerHP playerHP;
    private Coroutine attackCoroutine;
    private Coroutine penaltyCoroutine;
    private bool isAttacking;
    private bool isInputBlocked;
    private bool isTakingDamage;
    private bool isDead;
    private float playerHp;
    private float maxPlayerHp;

    public static PlayerSC Instance { get; private set; }
    public float CurrentHp => playerHp;
    public DeckManager Deck => deckManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        deckManager = GetComponent<DeckManager>();
        audios = GetComponent<Audios>();
        playerHP = GetComponent<PlayerHP>();

        if (PlayerData == null || PlayerData.PlayerList == null || PlayerData.PlayerList.Count == 0)
        {
            Debug.LogError("PlayerData가 비어 있습니다.", this);
            enabled = false;
            return;
        }

        PlayerDataSC.PlayerInfo playerInfo = PlayerData.PlayerList[0];
        deckManager.Initialize(playerInfo.Bullet_Setting);
        maxPlayerHp = Mathf.Max(0f, playerInfo.HP);
        playerHp = maxPlayerHp;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            darkColor = new Color(
                originalColor.r * 0.5f,
                originalColor.g * 0.5f,
                originalColor.b * 0.5f,
                originalColor.a);
        }
    }

    private void Start()
    {
        playerHP.UIInitialize(playerHp);
        UpdateBulletText();
    }

    private void Update()
    {
        if (isDead || isAttacking || isInputBlocked || GameManager.Instance == null)
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (GameManager.Instance.PenaltyCh)
            {
                penaltyCoroutine = StartCoroutine(PenaltyRoutine());
                return;
            }

            if (GameManager.Instance.EndGameCh)
            {
                return;
            }

            isAttacking = true;
            attackCoroutine = StartCoroutine(AttackLoop());
        }
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead && GameManager.Instance != null && !GameManager.Instance.EndGameCh)
        {
            if (deckManager.CylinderList.Count == 0)
            {
                deckManager.ReloadBullet();
                if (deckManager.CylinderList.Count == 0)
                {
                    break;
                }

                audios.PlayReload();
                SetBulletText("Reloading...");
                yield return new WaitForSeconds(penaltyWait);
                UpdateBulletText();

                if (GameManager.Instance.EndGameCh)
                {
                    break;
                }
            }

            if (targetEnemy == null)
            {
                changetarget();
            }

            if (TryAttackCurrentTarget())
            {
                audios.PlayShot();
                deckManager.ShotBullet();
                UpdateBulletText();
                yield return new WaitForSeconds(PlayerData.PlayerList[0].Shot_Delay);
            }
            else
            {
                yield return null;
            }
        }

        isAttacking = false;
        attackCoroutine = null;
    }

    public void PrepareForStage()
    {
        StopAttack();
        targetEnemy = null;
        deckManager.PrepareForNextStage();
        UpdateBulletText();
    }

    public void StopCombat()
    {
        StopAttack();

        if (penaltyCoroutine != null)
        {
            StopCoroutine(penaltyCoroutine);
            penaltyCoroutine = null;
        }

        isInputBlocked = false;
    }

    private void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
    }

    private IEnumerator PenaltyRoutine()
    {
        isInputBlocked = true;
        yield return new WaitForSeconds(penaltyWait);
        isInputBlocked = false;
        penaltyCoroutine = null;
    }

    public void AddBulletToDeck(int bulletId)
    {
        deckManager.AddBullet(bulletId);
    }

    public Dictionary<int, int> GetOwnedBulletCounts()
    {
        return deckManager.GetBulletCounts();
    }

    public bool RemoveBulletFromDeck(int bulletId)
    {
        bool removed = deckManager.RemoveBullet(bulletId);
        if (removed)
        {
            UpdateBulletText();
        }

        return removed;
    }

    public void RestoreFullHealth()
    {
        if (isDead)
        {
            return;
        }

        playerHp = maxPlayerHp;
        if (playerHP != null)
        {
            playerHP.UpdateHPBar(playerHp);
        }
    }

    public void changetarget()
    {
        targetEnemy = null;

        if (GameManager.Instance == null || GameManager.Instance.StageManager == null)
        {
            return;
        }

        foreach (Enemy enemy in GameManager.Instance.StageManager.EnemyList)
        {
            if (enemy != null)
            {
                targetEnemy = enemy;
                break;
            }
        }
    }

    private bool TryAttackCurrentTarget()
    {
        if (targetEnemy == null || deckManager.CylinderList.Count == 0 ||
            DataManager.Instance == null)
        {
            return false;
        }

        int bulletId = deckManager.CylinderList[0];
        if (!DataManager.Instance.TryGetBulletInfo(
                bulletId,
                out BulletDataSC.BulletInfo info))
        {
            return false;
        }

        float attackMultiplier = Mathf.Max(0f, PlayerData.PlayerList[0].attack_magnification);
        targetEnemy.Enemytakedamage(info.BulletDamage * attackMultiplier);
        return true;
    }

    public void Playertakedamage(float attackDamage)
    {
        if (isDead)
        {
            return;
        }

        playerHp -= attackDamage;
        playerHP.UpdateHPBar(Mathf.Max(0f, playerHp));

        if (!isTakingDamage && spriteRenderer != null)
        {
            StartCoroutine(Darken());
        }

        if (playerHp <= 0f)
        {
            Die();
        }
    }

    private IEnumerator Darken()
    {
        isTakingDamage = true;
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = darkColor;
            yield return new WaitForSeconds(0.0625f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.0625f);
        }

        isTakingDamage = false;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        StopCombat();
        GameManager.Instance.PlayerDie();
    }

    private void UpdateBulletText()
    {
        SetBulletText($"{deckManager.CylinderList.Count} / {DeckManager.CylinderCapacity}");
    }

    private void SetBulletText(string value)
    {
        if (BulletText != null)
        {
            BulletText.text = value;
        }
    }
}
