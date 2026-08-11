using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool EndGameCh;
    public bool PenaltyCh;
    public bool StageClear;
    public bool GameOver;
    public int countdown;
    private int enemyCount;

    public PlayerSC Player { get; private set; }
    [field: SerializeField] public StageManager StageManager { get; private set; }
    [SerializeField] private Text countdownText;
    [SerializeField] private GameObject gameoverui;
    [SerializeField] private GameObject stageclearui;
    private void Awake()
    {
        // 2. 씬이 시작될 때 인스턴스 할당
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Player = FindAnyObjectByType<PlayerSC>(); ;
    }

    private void Start()
    {
        enemyCount = StageManager.randomCount + 1;
        EndGameCh = false;
        PenaltyCh = true;
        StageClear = false;
        GameOver = false;
        StartCoroutine(CountdownCoroutine());
    }
    private IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = countdown; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
            
        }
        PenaltyCh = false;
        countdownText.text = "START!";

        yield return new WaitForSeconds(0.3f);

        countdownText.gameObject.SetActive(false);
    }

    public void PlayerDie()
    {
        GameOver = true;
        EndGame();
    }

    public void EnemyDie()
    {
        enemyCount--;
        if(enemyCount == 0)
        {
            StageClear = true;
            EndGame();
        }
    }

    public void EndGame()
    {
        EndGameCh = true;
        if(GameOver == true)
        {
            gameoverui.SetActive(true);
        }
        else if(StageClear == true)
        {
            stageclearui.SetActive(true);
        }
    }

}
