using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    private List<int> origin;
    private List<int> deck;
    private int[] cylinder = new int[6];
    private int room = 6;

    public GameObject bulletPrefab;
    public Transform bulletUIPoint;
    public GameObject bulletImage;
    private GameObject[] bulletUI = new GameObject[6];
    public TextMeshProUGUI fireDelayText;

    private bool isFire = false;
    public int fireDelay = 150;
    public int currDelay = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        origin = Player.Instance.defaultBullets;

        for(int i = 0; i < room; i++)
        {
            Vector3 bulletPoint = new Vector3(bulletUIPoint.position.x + i, bulletUIPoint.position.y, bulletUIPoint.position.z);
            bulletUI[i] = Instantiate(bulletImage, bulletPoint, Quaternion.identity);
        }
        
        Reload();
        fireDelayText.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
       fireDelayText.text = (fireDelay - currDelay).ToString();
    }

    void FixedUpdate()
    {
        if (currDelay >= fireDelay)
        {
            isFire = true;
        }
        if(!isFire) currDelay++;
    }

    public void FireBullet()
    {
        if (!isFire) return;

        for(int i = 0; i < room; i++)
        {
            GameObject newBullet = Instantiate(bulletPrefab);
            newBullet.SetActive(false);
            newBullet.GetComponent<Bullet>().Init(cylinder[i]);
            newBullet.SetActive(true);
        }

        Reload();
        currDelay = 0;
        isFire = false;

    }

    void Reload()
    {
        Shuffle();
        for(int i = 0; i < room; i++)
        {
            cylinder[i] = deck[i];
            bulletUI[i].GetComponent<BulletImage>().SetId(cylinder[i]);
        }
    }

    void Shuffle()
    {
        deck = origin.ToList();
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}
