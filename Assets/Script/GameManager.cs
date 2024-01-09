using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    enum CurrentScene
    {
        title,
        main
    }

    public class Data
    {
        public string name;
        public int money;
        public int startday;
        public int currentday;

        public void ReadData()
        {
            Debug.Log("name : " + name);
        }
    }
    string dataFileName = "data.json";

    private CurrentScene scene;

    public static GameManager instance = null;

    [SerializeField] private GameObject Lettuce;
    [SerializeField] private GameObject Spinach;
    [SerializeField] private GameObject Garlic;

    [SerializeField] private GameObject Microbe1;
    [SerializeField] private GameObject Microbe2;
    [SerializeField] private GameObject Microbe3;

    [SerializeField] private GameObject Promoter;
    [SerializeField] private GameObject Nutrients;
    [SerializeField] private GameObject Fertilizer;

    [SerializeField] private GameObject Seed_Lt;
    [SerializeField] private GameObject Seed_Sp;
    [SerializeField] private GameObject Seed_Gl;

    [SerializeField] private GameObject Field;

    [SerializeField] private GameObject Laboratory;
    [SerializeField] private GameObject Fertilizer_Facility;
    [SerializeField] private GameObject Storage;

    [SerializeField] private int GameMoney = 0;
    [SerializeField] private Dictionary<string, int>CropsLevel = new Dictionary<string, int>();
    [SerializeField] private Dictionary<string, int>Inventory = new Dictionary<string, int>();
    [SerializeField] private Dictionary<string, int> AuctionPrice = new Dictionary<string, int>();


    [SerializeField] private float onehour = 3600;
    [SerializeField] private float currentTime;
    [SerializeField] private float totalTime;

    DateTime dt = DateTime.Now;

    private int[] StartDay = new int[3];        //연,월,일을 저장
    private int CurrentDay;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
        scene = CurrentScene.title;
    }

    // Start is called before the first frame update
    void Start()
    {
        CropsLevel.Add("lettuce", 1);
        CropsLevel.Add("spinach", 1);
        CropsLevel.Add("garlic", 1);

        Inventory.Add("promoter", 0);
        Inventory.Add("nutrients", 0);
        Inventory.Add("Rertilizer", 0);

        AuctionPrice.Add("lettuce", 0);
        AuctionPrice.Add("spinach", 0);
        AuctionPrice.Add("garlic", 0);
        init();
        DateSet();
    }

    // Update is called once per frame
    void Update()
    {
        OnClickConstruction();
    }

    private void SceneManager()
    {
        switch(scene)
        {
            case CurrentScene.title:
                break;

            case CurrentScene.main:
                break;
        }
    }

    private void init()
    {
        Data data = new Data();
        string filePath = Path.Combine(Application.persistentDataPath, "userdata.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(jsonData);
            Debug.Log("name: " + data.name);
        }
        else
        {
            string saveData = JsonUtility.ToJson(data, true);
            filePath = Application.persistentDataPath + "/userdata.json";
            File.WriteAllText(filePath, saveData);
        }
    }

    private void AuctionPricing()
    {
        AuctionPrice["lettuce"] = UnityEngine.Random.Range(30, 150);
        AuctionPrice["spinach"] = UnityEngine.Random.Range(200, 900);
        AuctionPrice["garlic"] = UnityEngine.Random.Range(500, 2500);
    }

    private void TakeMoney(int money)
    {
        GameMoney += money;
    }

    private void CropsLevelUp(string name)
    {
        CropsLevel[name] += 1;
    }

    private void AddInventory(string name)
    {
        Inventory[name] += 1;
    }

    public int[] Timer()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= onehour)
        {
            currentTime = 0;
        }

        int hour = (int)(currentTime / 3600);
        int minute = (int)((currentTime % 3600) / 60);
        int seconds = (int)(currentTime % 60);

        int[] timeValue = new int[] { hour, minute, seconds };

        return timeValue;
    }

    private void DateSet()
    {
        StartDay[0] = dt.Year;
        StartDay[1] = dt.Month;
        StartDay[2] = dt.Day;
        CurrentDay = dt.Day;
    }

    private void OnClickConstruction()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == Laboratory)
                {
                    Debug.Log("연구소");
                }

                else if(hit.collider.gameObject == Fertilizer_Facility)
                {

                }

                else if(hit.collider.gameObject == Storage)
                {

                }
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {

        }
    }
}
