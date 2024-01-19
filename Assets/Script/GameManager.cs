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

    /// <summary>
    /// ui에서 설정한 값에 따라 아이템 사용, 작물 심기 상태 변경
    /// </summary>

    private class Data
    {
        public string name;
        public int money;
        public int startday;
        public int currentday;
    }
    Data data;
    string dataFileName = "data.json";

    private CurrentScene scene;

    public static GameManager instance = null;
    private UIManager ui;
    private FieldManager fm;

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

    [SerializeField] private int gameMoney;
    [SerializeField] private bool useItem;
    public bool UseItem
    {
        get { return useItem; }
        set { useItem = value; }
    }
    public int GameMoney
    {
        get { return gameMoney; }
        set { gameMoney = value; }
    }
    [SerializeField] private Dictionary<string, int>CropsLevel = new Dictionary<string, int>();
    [SerializeField] private Dictionary<string, int>Inventory = new Dictionary<string, int>();
    [SerializeField] private Dictionary<string, int> AuctionPrice = new Dictionary<string, int>();

    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(Inventory);
    }

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

        ui = GetComponent<UIManager>();
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
        Inventory.Add("lettuce seed", 1);
        Inventory.Add("spinach seed", 0);
        Inventory.Add("garlic seed", 0);

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
        InstanceField();
        InventoryManager();
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
        data = new Data();
        string filePath = Path.Combine(Application.persistentDataPath, "userdata.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(jsonData);
            Debug.Log(data.startday);
        }
        else
        {
            string saveData = JsonUtility.ToJson(data, true);
            filePath = Application.persistentDataPath + "/userdata.json";
            File.WriteAllText(filePath, saveData);
        }

        GameMoney = 0;
        ui.ui.Name = data.name;
        ui.ui.Money = data.money;
    }

    private void InstanceField()
    {
        Vector3 pos = GameObject.Find("ParentField").transform.position;

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                pos += Vector3.right;
                if(!IsObjectAtPosition(pos))
                {
                    GameObject cloneField = Instantiate(Field, pos, Quaternion.identity);
                    cloneField.transform.parent = GameObject.Find("ParentField").transform;
                }
            }
            pos += Vector3.down;
            pos += Vector3.left * 3;
        }
    }
    
    /// <summary>
    /// 위치에 오브젝트가 존재하는지 여부
    /// 3d로 할 때에도 마찬가지로 사용 가능할듯
    /// </summary>
    private bool IsObjectAtPosition(Vector3 pos)
    {
        Vector2 pos2D = new Vector2(pos.x, pos.y);
        Collider2D[] colliders = Physics2D.OverlapPointAll(pos2D);
        if (colliders.Length > 0)
        {
            return true;
        }
        return false;
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

    public void AddInventory(string name)
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
    }

    /// <summary>
    /// 인벤토리에 각 key값의 아이템이 1개라도 들어온다면 인벤토리에 인스턴스 시키는 형식으로 함.
    /// </summary>
    private void InventoryManager()
    {
        foreach(var item in Inventory)
        {
            if(item.Value == 0)
            {

            }
        }
    }

    public void InstanceInventoryItem()
    {
        foreach (var item in Inventory)
        {
            if (item.Value == 0)
            {
                Debug.Log(item.Value + item.Key);
            }
        }
    }

    public GameObject InstanceInven(string name)
    {
        switch (name)
        {
            case "lettuce seed":
                return Seed_Lt;

            default:
                return null;
        }
    }
}
