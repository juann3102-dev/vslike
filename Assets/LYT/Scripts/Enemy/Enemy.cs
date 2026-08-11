using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DeckManager), typeof(EnemyHealth), typeof(Audios))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private Text BulletText;

    private EnemyHealth health;
    private Audios audios;
    private EnemyInfo myData;
    private DeckManager deckManager;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color darkColor;
    private float currentHp;
    private float damageMultiplier = 1f;
    private float startAttackRandomMultiplier;
    private bool initialized;
    private bool attackStarted;
    private bool isDead;

    public event Action<Enemy> OnDie;

    private void Awake()
    {
        audios = GetComponent<Audios>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<EnemyHealth>();
        deckManager = GetComponent<DeckManager>();
        startAttackRandomMultiplier = UnityEngine.Random.Range(0.5f, 1.5f);

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

    public bool Initialize(
        EnemyInfo info,
        float hpMultiplier,
        float stageDamageMultiplier)
    {
        if (info == null || info.Bullet_Setting == null || info.Bullet_Setting.Length == 0)
        {
            Debug.LogError("적 초기화 데이터가 올바르지 않습니다.", this);
            return false;
        }

        myData = info;
        damageMultiplier = Mathf.Max(0f, stageDamageMultiplier);
        deckManager.Initialize(myData.Bullet_Setting);
        currentHp = myData.Enemy_HP * Mathf.Max(0.01f, hpMultiplier);
        health.UIInitialize(currentHp);
        UpdateBulletText();
        initialized = true;
        return true;
    }

    private void Update()
    {
        if (!initialized || attackStarted || GameManager.Instance == null)
        {
            return;
        }

        if (!GameManager.Instance.PenaltyCh && !GameManager.Instance.EndGameCh)
        {
            attackStarted = true;
            StartCoroutine(EnemyOnAttack());
        }
    }

    private IEnumerator EnemyOnAttack()
    {
        yield return new WaitForSeconds(
            myData.Enemy_Start_Shot * startAttackRandomMultiplier);

        while (!isDead && GameManager.Instance != null && !GameManager.Instance.EndGameCh)
        {
            if (deckManager.CylinderList.Count == 0)
            {
                deckManager.ReloadBullet();
                if (deckManager.CylinderList.Count == 0)
                {
                    yield break;
                }

                audios.PlayReload();
                SetBulletText("Reloading...");
                yield return new WaitForSeconds(1.25f);

                if (GameManager.Instance == null || GameManager.Instance.EndGameCh)
                {
                    yield break;
                }

                UpdateBulletText();
            }

            EnemyAttack();
            audios.PlayShot();
            deckManager.ShotBullet();
            UpdateBulletText();
            yield return new WaitForSeconds(myData.Enemy_Delay);
        }
    }

    private void EnemyAttack()
    {
        if (deckManager.CylinderList.Count == 0 || PlayerSC.Instance == null ||
            DataManager.Instance == null)
        {
            return;
        }

        int bulletId = deckManager.CylinderList[0];
        if (!DataManager.Instance.TryGetBulletInfo(
                bulletId,
                out BulletDataSC.BulletInfo info))
        {
            return;
        }

        float finalDamage = info.BulletDamage * damageMultiplier;
        PlayerSC.Instance.Playertakedamage(finalDamage);
    }

    public void Enemytakedamage(float attackDamage)
    {
        if (isDead)
        {
            return;
        }

        currentHp -= attackDamage;
        health.UpdateHPBar(Mathf.Max(0f, currentHp));

        if (spriteRenderer != null)
        {
            StartCoroutine(Darken());
        }

        if (currentHp <= 0f)
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

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        OnDie?.Invoke(this);
        Destroy(gameObject);
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
