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
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            for (int i = Mathf.Max(0, countdown); i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            countdownText.text = "START!";
            yield return new WaitForSeconds(0.3f);
            countdownText.gameObject.SetActive(false);
        }

        BeginStage();
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
        SetActive(stageclearui, true);

        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager가 없어 보상을 생성할 수 없습니다.", this);
            CompleteReward(null);
            return;
        }

        List<BulletDataSC.BulletInfo> choices =
            DataManager.Instance.GetRewardChoices(3);
        rewardUI.Show(CurrentStageNumber, choices, CompleteReward);
    }

    private void CompleteReward(BulletDataSC.BulletInfo selectedBullet)
    {
        if (selectedBullet != null)
        {
            Player.AddBulletToDeck(selectedBullet.id);
        }

        StartCoroutine(StartNextStage());
    }

    private IEnumerator StartNextStage()
    {
        yield return null;
        CurrentStageNumber++;
        stageTransitionInProgress = false;
        BeginStage();
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
}
