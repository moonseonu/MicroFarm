using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

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
    private class FieldData
    {
        public int field_num;
        public string type;
        public string microbe;
        public DateTime start_time;
        public int count;
    }
    private class QueueData
    {
        public string microbe;
        public DateTime start_time;
    }

    private class Data
    {
        public string name;
        public int money;
        public int currentday = 0;
        public int storage_count;
        public int fieldCount;

        public float basePrice = 100.0f; // 초기 주가
        public float meanChange = 0.0f; // 주가 변동의 평균
        public float stdDeviation = 5.0f; // 주가 변동의 표준 편차

        public Dictionary<string, int> CropsLevel = new Dictionary<string, int>();
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
        public Dictionary<string, int> AuctionPrice = new Dictionary<string, int>();
        public Dictionary<string, int> SeedPrice = new Dictionary<string, int>();
        public Dictionary<string, int> CropWarehouse = new Dictionary<string, int>();
        public Dictionary<string, int> Microbe = new Dictionary<string, int>();
        public Dictionary<string, int> Production_Microbe = new Dictionary<string, int>();
        public Dictionary<string, Queue<int>> stockData = new Dictionary<string, Queue<int>>();
        public List<FieldData> fieldDatas = new List<FieldData>();
        public List<QueueData> microbeQueue = new List<QueueData>();
    }
    Data data;

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

    [SerializeField] private GameObject Fertilizer;

    [SerializeField] private GameObject Seed_Lt;
    [SerializeField] private GameObject Seed_Sp;
    [SerializeField] private GameObject Seed_Gl;
    [SerializeField] private GameObject Empty_Slot;

    [SerializeField] private GameObject Field;

    [SerializeField] private GameObject Laboratory;
    [SerializeField] private GameObject Fertilizer_Facility;
    [SerializeField] private GameObject Storage;

    [SerializeField] private int gameMoney;
    [SerializeField] private string useItem = "";

    [SerializeField] private List<int[]> MicrobesPattern = new List<int[]>
    {
        new int[]{1, 0, 1, 
                  0, 1, 0,
                  1, 0, 1},

        new int[]{1, 0, 0,
                  0, 1, 0,
                  0, 0, 1},

        new int[]{0, 1, 0,
                  1, 1, 1,
                  0, 1, 0}
    };

    private int UsedPattern = -1;
    /// <summary>
    /// 좌표 만들기용 딕셔너리
    /// </summary>

    public Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

    public int Storage_Count
    {
        get { return data.storage_count; }
        set { data.storage_count = value; }
    }

    public Dictionary<string, bool>cultivate = new Dictionary<string, bool>();

    public string usedName = "";
    public int GameMoney
    {
        get { return gameMoney; }
        set { gameMoney = value; }
    }

    [SerializeField] private float onehour = 3600;
    [SerializeField] private float currentTime;
    [SerializeField] private float totalTime;

    DateTime dt = DateTime.Now;

    private int[] StartDay = new int[3];        //연,월,일을 저장

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);

        ui = GetComponent<UIManager>();
        ui.UI_Init();
        scene = CurrentScene.title;
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
        DateSet();
    }

    // Update is called once per frame
    void Update()
    {
        OnClickConstruction();
        UpdatePlanting();
        SaveData();
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
        string filePath = Path.Combine(Application.persistentDataPath, "userdata.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            data = JsonConvert.DeserializeObject<Data>(jsonData);
        }
        else
        {
            data = new Data();

            data.money = 5000;

            data.Inventory.Add("lettuce", 0);
            data.Inventory.Add("spinach", 0);
            data.Inventory.Add("garlic", 0);

            data.AuctionPrice.Add("lettuce", 100);
            data.AuctionPrice.Add("spinach", 0);
            data.AuctionPrice.Add("garlic", 0);

            data.SeedPrice.Add("lettuce", 10);

            data.CropWarehouse.Add("lettuce", 0);
            data.CropWarehouse.Add("spinach", 0);
            data.CropWarehouse.Add("garlic", 0);

            data.Microbe.Add("microbe1", 1);
            data.Microbe.Add("microbe2", 0);
            data.Microbe.Add("microbe3", 0);

            data.Microbe.Add("Sample1", 1);
            data.Microbe.Add("Sample2", 0);
            data.Microbe.Add("Sample3", 0);

            data.Production_Microbe.Add("product1", 0);

            data.storage_count = 50;
            data.fieldCount = 9;

            data.stockData["lettuce"] = new Queue<int>();
            while (data.stockData["lettuce"].Count < 5)
            {
                if (data.stockData["lettuce"].Count == 5)
                {
                    data.stockData["lettuce"].Enqueue(100);
                }
                data.stockData["lettuce"].Enqueue(0);
            }
        }
        cultivate.Add("lettuce", false);
        cultivate.Add("microbe1", false);
        cultivate.Add("fertilizer", false);
        if (data.currentday != dt.Day)
        {
            if(data.currentday == 0)
            {
                data.currentday = dt.Day;
                AuctionPricing();
            }

            else
            {
                int difference = dt.Day - data.currentday;
                while (difference > 0)
                {
                    AuctionPricing();
                    difference--;
                }
                //ui.Auction_Init(data.AuctionPrice);
            }
        }

        keyValuePairs.Add("Sample1", "microbe1");
        keyValuePairs.Add("Sample2", "microbe2");
        keyValuePairs.Add("Sample3", "microbe3");

        ui.ui.Name = data.name;
        ui.ui.Money = data.money;
        GameMoney = data.money;

        InitInventory();
        InitStorage();
        InstanceField();
        Microbe_Init();
    }

    private void SaveData()
    {
        data.money = GameMoney;
        string filePath = Path.Combine(Application.persistentDataPath, "userdata.json");
        string saveData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, saveData);
    }

    private void InstanceField()
    {
        Vector3 pos = GameObject.Find("ParentField").transform.position;
        int fieldNum = 0;
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                pos += Vector3.right;
                if(!IsObjectAtPosition(pos))
                {
                    GameObject cloneField = Instantiate(Field, pos, Quaternion.identity);
                    cloneField.transform.parent = GameObject.Find("ParentField").transform;
                    fm = cloneField.GetComponent<FieldManager>();
                    FieldData ExistFieldData = data.fieldDatas.Find(ExistFieldData => ExistFieldData.field_num == fieldNum);

                    if(ExistFieldData == null)
                    {
                        FieldData NewData = new FieldData { field_num = fieldNum , type = "", microbe = ""};
                        data.fieldDatas.Add(NewData);
                        fm.Init(fieldNum);
                    }

                    else if(ExistFieldData != null && ExistFieldData.type == "" && ExistFieldData.microbe == "")
                    {
                        fm.Init(fieldNum);
                    }

                    else if(ExistFieldData != null && ExistFieldData.type != "" && ExistFieldData.microbe != "")
                    {
                        TimeSpan time = DateTime.Now - ExistFieldData.start_time;
                        fm.Init(fieldNum, ExistFieldData.type, ExistFieldData.microbe ,time);
                    }

                    else if (ExistFieldData != null && ExistFieldData.type != "" && ExistFieldData.microbe == "")
                    {
                        TimeSpan time = DateTime.Now - ExistFieldData.start_time;
                        fm.Init(fieldNum, ExistFieldData.type, ExistFieldData.microbe, time);
                    }

                    fieldNum++;
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

        DequeuePrice("lettuce");

        //data.AuctionPrice["spinach"] = UnityEngine.Random.Range(200, 900);
        //data.AuctionPrice["garlic"] = UnityEngine.Random.Range(500, 2500);
    }

    private void DequeuePrice(string name)
    {
        data.AuctionPrice[name] = (int)GeneratePriceFluctuations();
        data.stockData[name].Enqueue(data.AuctionPrice[name]);
        if (data.stockData[name].Count > 4)
        {
            data.stockData[name].Dequeue();
        }
    }

    public Queue<int> GetStockPrice(string name)
    {
        return data.stockData[name];
    }

    private void TakeMoney(int money)
    {
        GameMoney += money;
    }

    private void CropsLevelUp(string name)
    {
        data.CropsLevel[name] += 1;
    }

    public void AddInventory(string name)
    {
        data.Inventory[name] += 1;
        InventoryManager();
    }

    public void UseInventory(string name)
    {
        data.Inventory[name] -= 1;
        ui.UseItem(name, 1);
    }

    public int Inventory_Count(string name)
    {
        return data.Inventory[name];
    }

    public int Microbe_Count(string name)
    {
        return data.Microbe[name];
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
        data.currentday = dt.Day;
    }

    private void OnClickConstruction()
    {
        if (!ui.isMenuOpen)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject == Laboratory)
                        {
                            ui.Laboratory_Open();
                        }

                        else if (hit.collider.gameObject == Fertilizer_Facility)
                        {

                        }

                        else if (hit.collider.gameObject == Storage)
                        {
                            ui.StorageInstance();
                        }

                        else
                        {
                            for(int i = 0; i < 9; i++)
                            {
                                GameObject field = GameObject.Find("Construction").transform.Find("ParentField").GetChild(i).gameObject;
                                if(hit.collider.gameObject == field)
                                {
                                    fm = field.GetComponent<FieldManager>();
                                    if (!fm.isGrowing)
                                    {
                                        if (fm.isHarvest)
                                        {
                                            FieldData ExistData = data.fieldDatas.Find(ExistData => ExistData.field_num == i);
                                            if (ExistData != null)
                                            {
                                                fm.harvesting(ExistData.type);
                                            }
                                        }
                                        else
                                        {
                                            ui.CultivationMenu_Open();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdatePlanting()
    {
        if (useItem != "")
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

                    if (hit.collider != null)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            GameObject field = GameObject.Find("Construction").transform.Find("ParentField").GetChild(i).gameObject;
                            if (hit.collider.gameObject == field)
                            {
                                fm = field.GetComponent<FieldManager>();
                                if (!fm.isGrowing)
                                {
                                    if (!fm.isHarvest)
                                    {
                                        if(useItem != "microbe1")
                                        {
                                            if (data.Inventory[useItem] > 0)
                                            {

                                                if (UsedPattern != -1)
                                                {
                                                    Debug.Log(UsedPattern);
                                                    fm.cultivation(useItem, MicrobesPattern[UsedPattern]);
                                                }
                                                else
                                                {
                                                    fm.cultivation(useItem);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (data.Microbe[useItem] > 0)
                                            {
                                                fm.cultivation(useItem);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        FieldData ExistData = data.fieldDatas.Find(ExistData => ExistData.field_num == i);
                                        if (ExistData != null)
                                        {
                                            fm.harvesting(ExistData.type);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void Planting(string crop)
    {
        switch (crop)
        {
            case "lettuce":
                if (data.Inventory[crop] > 0)
                {
                    useItem = crop;
                }
                break;

            case "microbe1":
                if (data.Microbe[crop] > 0)
                    useItem = crop;
                break;

            case "close":
                if(useItem == "microbe1")
                {
                    int[] useMicrobe = new int[9];
                    for(int i = 0; i < data.fieldDatas.Count; i++)
                    {
                        if (data.fieldDatas[i].microbe == useItem)
                        {
                            useMicrobe[i] = 1;
                        }
                        else
                        {
                            useMicrobe[i] = 0;
                        }
                    }
                    for(int j = 0; j < MicrobesPattern.Count; j++)
                    {
                        if (MicrobesPattern[j].Length != useMicrobe.Length)
                        {
                            continue;
                        }
                        bool isMatching = true;
                        for(int k = 0; k < useMicrobe.Length; k++)
                        {
                            if (useMicrobe[k] != MicrobesPattern[j][k])
                            {
                                isMatching = false;
                                break;
                            }
                        }
                        if (isMatching)
                        {
                            UsedPattern = j;
                            break;
                        }
                    }    

                }
                useItem = "";
                break;
        }
    }

    /// <summary>
    /// 인벤토리에 각 key값의 아이템이 1개라도 들어온다면 인벤토리에 인스턴스 시키는 형식으로 함.
    /// </summary>
    private void InventoryManager()
    {
        foreach(var item in data.Inventory)
        {
            if (item.Key == "lettuce")
            {
                ui.AddInventory(Seed_Lt, item.Value, false);
            }
        }
    }

    private void InitInventory()
    {
        foreach (var item in data.Inventory)
        {
            if (item.Key == "lettuce")
            {
                ui.AddInventory(Seed_Lt, item.Value, true);
            }
        }
    }

    private void InitStorage()
    {
        foreach (var item in data.CropWarehouse)
        {

            ui.AddStorage(item.Key, item.Value, true);
        }
    }

    public void IsUsedItem(string name)
    {
        usedName = name;
    }

    public void HarvestCrops(string name, int yield)
    {
        data.CropWarehouse[name] += 1 * yield;
        ui.AddStorage(name, 1 * yield, false);

        int random = UnityEngine.Random.Range(0, 1);
        if(random == 0)
        {
            data.Microbe["Sample1"] += 1;
            ui.Update_Lab("microbe1", 1);
        }
    }

    public void Selling(string name, int num)
    {
        if (data.CropWarehouse[name] > 0)
        {
            ui.UpdateStorage(name, num);
            data.CropWarehouse[name] -= num;
            GameMoney += num * data.AuctionPrice[name];
            ui.ui.Money = GameMoney;
        }
    }

    public void Buying(string name, int num)
    {
        if (GameMoney >= data.AuctionPrice[name])
        {
            AddInventory(name);
            GameMoney -= num * data.SeedPrice[name];
            ui.ui.Money = GameMoney;
        }
    }

    public TimeSpan Growth_Time_Manager(string name, int num)
    {
        FieldData ExistData = data.fieldDatas.Find(ExistData => ExistData.field_num == num);
        if (ExistData != null && ExistData.type == "")
        {
            ExistData.type = name;

            ExistData.start_time = DateTime.Now;
        }

        TimeSpan timeSpan = DateTime.Now - ExistData.start_time;
        return timeSpan;
    }

    public void FieldDataInit(int num)
    {
        FieldData ExistData = data.fieldDatas.Find(ExistData => ExistData.field_num == num);

        if(ExistData != null)
        {
            ExistData.type = "";
            ExistData.microbe = "";
            ExistData.start_time = DateTime.MinValue;
        }
    }

    //public void CultivationManager(string name)
    //{
    //    switch (name)
    //    {
    //        case "lettuce":
    //            if (!cultivate["lettuce"])
    //            {
    //                Debug.Log(cultivate["lettuce"]);
    //                cultivate["lettuce"] = true;
    //            }
    //            else
    //            {
    //                Debug.Log(cultivate["lettuce"]);
    //                cultivate["lettuce"] = false;
    //            }
    //            break;

    //        case "microbe1":
    //            if (!cultivate["microbe1"])
    //            {
    //                Debug.Log(cultivate["microbe1"]);
    //                cultivate["microbe1"] = true;
    //            }
    //            else
    //            {
    //                Debug.Log(cultivate["microbe1"]);
    //                cultivate["microbe1"] = false;
    //            }
    //            break;

    //        case "fertilizer":

    //            break;
    //    }
    //}

    public void CultivatonMenu_Close()
    {
        foreach(var key in  cultivate.Keys)
        {
            cultivate[key] = false;
        }
    }

    private void Microbe_Init()
    {
        //미생물 배양중인거 있을 때
        ///
        /// 나우를 박았을 경우 대기열 4개일 때
        /// 40분 나우 , null, null, null일 텐데
        /// 15분 뒤에 접속을 하면
        /// 첫번째거 클리어 남은 값 5분 현재시간은 55분이지만 == 50분 나우 ,null ,null
        ///
        if (data.microbeQueue.Count != 0)
        {
            TimeSpan timeSpan;
            timeSpan = DateTime.Now - data.microbeQueue[0].start_time;

            if (data.microbeQueue[0].microbe == "Sample1")
            {
                if (timeSpan.TotalSeconds >= 60)
                {
                    data.Microbe[keyValuePairs[data.microbeQueue[0].microbe]]++;
                    timeSpan -= TimeSpan.FromSeconds(60);
                    data.microbeQueue.RemoveAt(0);
                    if(data.microbeQueue.Count > 0)
                        CultureInit(timeSpan);
                }
            }
        }
        for(int i = 0; i < data.microbeQueue.Count; i++)
        {
            ui.InitProduction_Microbe(data.microbeQueue[i].microbe);
        }
        //ui.Laboratory_Manager(data.Microbe["microbe1"], data.Microbe["microbe2"], data.Microbe["microbe3"], 
        //    data.Microbe["Sample1"], data.Microbe["Sample2"], data.Microbe["Sample3"], 0);

        ui.Laboratory_Manager(data.Microbe["microbe1"], data.Microbe["Sample1"], 0);
    }

    private void CultureInit(TimeSpan ts)
    {
        if (data.microbeQueue[0].microbe == "Sample1")
        {
            if (ts.TotalSeconds >= 60)
            {
                data.Microbe[keyValuePairs[data.microbeQueue[0].microbe]]++;
                data.microbeQueue.RemoveAt(0);
                if(data.microbeQueue.Count > 0)
                {
                    ts -= TimeSpan.FromSeconds(60);
                    CultureInit(ts);
                }
            }

            else if (ts.TotalSeconds < 60 && ts.TotalSeconds > 0)
            {
                data.microbeQueue[0].start_time = DateTime.Now.Subtract(ts);
            }
        }

        else
        {

        }
    }

    public bool MicrobeQueueManager(GameObject type)
    {
        if (data.Microbe[type.name] > 0 && data.microbeQueue.Count < 11)
        {
            QueueData NewData = new QueueData { microbe =  type.name};
            data.microbeQueue.Add(NewData);
            data.Microbe[type.name]--;

            return true;
        }

        else return false;
    }

    public void UseMicrobe(string microbe, int num)
    {
        if (data.Microbe[microbe] > 0)
        {
            data.Microbe[microbe]--;
            FieldData ExistData = data.fieldDatas.Find(ExistData => ExistData.field_num == num);
            if (ExistData != null)
            {
                ExistData.microbe = microbe;
                ui.Update_Lab(microbe, -1);
            }
        }
    }

    public void UpdateCulture(string type)
    {
        QueueData ExistData = data.microbeQueue.Find(ExistData => ExistData.microbe == type);
        if(ExistData != null)
        {
            if(ExistData == data.microbeQueue[0])
            {
                DateTime defaultDateTime = new DateTime();
                if(ExistData.start_time == defaultDateTime)
                {
                    ExistData.start_time = DateTime.Now;
                }
                TimeSpan timeSpan = DateTime.Now - ExistData.start_time;

                if(timeSpan >= TimeSpan.FromSeconds(60))
                {
                    data.Microbe[keyValuePairs[type]]++;
                    data.microbeQueue.RemoveAt(0);
                    //ui.Laboratory_Manager(keyValuePairs[type], 1);
                    ui.ComplieteCulture(keyValuePairs[type], 1);
                }
            }
        }
    }

    float GeneratePriceFluctuations()
    {
        DateTime currentDate = DateTime.Now.Date;
        float currentPrice = (float)data.AuctionPrice["lettuce"];

        float randomValue = Mathf.Sqrt(-2.0f * Mathf.Log(UnityEngine.Random.value)) *
                            Mathf.Cos(2.0f * Mathf.PI * UnityEngine.Random.value);

        float priceChange = data.meanChange + randomValue * data.stdDeviation;

        currentPrice += priceChange;
        currentDate = currentDate.AddDays(1);

        return currentPrice;
    }
}
