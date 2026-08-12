using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool EndGameCh;
    public bool PenaltyCh;
    public bool StageClear;
    public bool GameOver;
    public int countdown = 3;

    public int CurrentStageNumber { get; private set; } = 1;
    public PlayerSC Player { get; private set; }
    [field: SerializeField] public StageManager StageManager { get; private set; }

    [SerializeField] private Text countdownText;
    [SerializeField] private GameObject gameoverui;
    [SerializeField] private GameObject stageclearui;

    private BulletRewardUI rewardUI;
    private Text stageText;
    private bool stageTransitionInProgress;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Player = FindAnyObjectByType<PlayerSC>();
        rewardUI = GetComponent<BulletRewardUI>();
        if (rewardUI == null)
        {
            rewardUI = gameObject.AddComponent<BulletRewardUI>();
        }
    }

    private void Start()
    {
        if (Player == null || StageManager == null)
        {
            Debug.LogError("GameManager의 Player 또는 StageManager 참조가 없습니다.", this);
            enabled = false;
            return;
        }

        EndGameCh = true;
        PenaltyCh = true;
        StageClear = false;
        GameOver = false;
        SetActive(gameoverui, false);
        SetActive(stageclearui, false);
        EnsureStageText();
        UpdateStageText();
        StageManager.Initialize(this);
        StartCoroutine(StartRun());
    }

    private void Update()
    {
        if (GameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartRun();
        }
    }

    private IEnumerator StartRun()
    {
        yield return StartCoroutine(ShowCountdownAndBeginStage());
    }

    private IEnumerator ShowCountdownAndBeginStage()
    {
        if (countdownText == null)
        {
            BeginStage();
            yield break;
        }

        countdownText.gameObject.SetActive(true);
        for (int i = Mathf.Max(0, countdown); i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "START!";
        BeginStage();
        yield return new WaitForSeconds(0.3f);
        countdownText.gameObject.SetActive(false);
    }

    private void BeginStage()
    {
        StageClear = false;
        EndGameCh = true;
        PenaltyCh = true;
        SetActive(stageclearui, false);

        Player.PrepareForStage();
        if (!StageManager.StartStage(CurrentStageNumber))
        {
            Debug.LogError($"스테이지 {CurrentStageNumber} 생성에 실패했습니다.", this);
            GameOver = true;
            EndGame();
            return;
        }

        EndGameCh = false;
        PenaltyCh = false;
        Player.changetarget();
    }

    public void PlayerDie()
    {
        if (GameOver)
        {
            return;
        }

        GameOver = true;
        StageClear = false;
        EndGame();
    }

    public void EnemyDie()
    {
        if (GameOver || stageTransitionInProgress || StageManager.EnemyList.Count > 0)
        {
            return;
        }

        stageTransitionInProgress = true;
        StageClear = true;
        EndGameCh = true;
        PenaltyCh = true;
        Player.StopCombat();
        Player.RestoreFullHealth();
        SetActive(stageclearui, true);

        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager가 없어 보상을 생성할 수 없습니다.", this);
            CompleteReward(null);
            return;
        }

        List<BulletRewardChoice> choices =
            DataManager.Instance.GetRewardChoices(3);
        List<OwnedBulletChoice> ownedBullets =
            DataManager.Instance.GetOwnedBulletChoices(
                Player.GetOwnedBulletCounts());
        rewardUI.Show(
            CurrentStageNumber,
            choices,
            ownedBullets,
            CompleteReward);
    }

    private void CompleteReward(BulletRewardChoice selectedChoice)
    {
        if (selectedChoice != null &&
            selectedChoice.Action == BulletRewardAction.AddBullet &&
            selectedChoice.Bullet != null)
        {
            Player.AddBulletToDeck(selectedChoice.Bullet.id);
        }
        else if (selectedChoice != null &&
                 selectedChoice.Action == BulletRewardAction.RemoveBullet &&
                 selectedChoice.Bullet != null &&
                 !Player.RemoveBulletFromDeck(selectedChoice.Bullet.id))
        {
            Debug.LogWarning(
                $"제거할 총알을 찾지 못했습니다. ID={selectedChoice.Bullet.id}",
                this);
        }

        StartCoroutine(StartNextStage());
    }

    private IEnumerator StartNextStage()
    {
        yield return null;
        CurrentStageNumber++;
        UpdateStageText();
        stageTransitionInProgress = false;
        SetActive(stageclearui, false);
        yield return StartCoroutine(ShowCountdownAndBeginStage());
    }

    public void EndGame()
    {
        EndGameCh = true;
        PenaltyCh = true;
        Player?.StopCombat();
        rewardUI?.Hide();

        if (GameOver)
        {
            SetActive(gameoverui, true);
            Text gameOverText = gameoverui != null ? gameoverui.GetComponent<Text>() : null;
            if (gameOverText != null && !gameOverText.text.Contains("R"))
            {
                gameOverText.text += "\nR 키로 다시 시작";
            }
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private static void SetActive(GameObject target, bool value)
    {
        if (target != null)
        {
            target.SetActive(value);
        }
    }

    private void EnsureStageText()
    {
        if (stageText != null)
        {
            return;
        }

        Canvas screenCanvas = countdownText != null
            ? countdownText.GetComponentInParent<Canvas>()
            : null;
        if (screenCanvas == null)
        {
            Debug.LogWarning("스테이지 표시를 배치할 화면 Canvas를 찾지 못했습니다.", this);
            return;
        }

        GameObject stageTextObject = new GameObject(
            "Stage Text",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text),
            typeof(Outline));
        stageTextObject.transform.SetParent(screenCanvas.transform, false);

        RectTransform rect = stageTextObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(40f, -30f);
        rect.sizeDelta = new Vector2(500f, 80f);

        stageText = stageTextObject.GetComponent<Text>();
        stageText.font = countdownText.font != null
            ? countdownText.font
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        stageText.fontSize = 42;
        stageText.fontStyle = FontStyle.Bold;
        stageText.alignment = TextAnchor.UpperLeft;
        stageText.color = Color.white;
        stageText.raycastTarget = false;

        Outline outline = stageTextObject.GetComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
        outline.effectDistance = new Vector2(2f, -2f);
    }

    private void UpdateStageText()
    {
        if (stageText != null)
        {
            stageText.text = $"STAGE {CurrentStageNumber}";
        }
    }
}
