using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;

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
    private class Data
    {
        public string name;
        public int money;
        public int startday;
        public int currentday;
        public int storage_count;

        public Dictionary<string, int> CropsLevel = new Dictionary<string, int>();
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
        public Dictionary<string, int> AuctionPrice = new Dictionary<string, int>();
        public Dictionary<string, int> SeedPrice = new Dictionary<string, int>();
        public Dictionary<string, int> CropWarehouse = new Dictionary<string, int>();
        public Dictionary<string, int> Microbe = new Dictionary<string, int>();

        public List<FieldData> fieldDatas = new List<FieldData>();
        public int fieldCount;
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
    [SerializeField] private GameObject Empty_Slot;

    [SerializeField] private GameObject Field;

    [SerializeField] private GameObject Laboratory;
    [SerializeField] private GameObject Fertilizer_Facility;
    [SerializeField] private GameObject Storage;

    [SerializeField] private int gameMoney;
    [SerializeField] private bool useItem;
    public int Storage_Count
    {
        get { return data.storage_count; }
        set { data.storage_count = value; }
    }

    public Dictionary<string, bool>cultivate = new Dictionary<string, bool>();

    public string usedName = "";
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
    //[SerializeField] private Dictionary<string, int>CropsLevel = new Dictionary<string, int>();
    //[SerializeField] private Dictionary<string, int>Inventory = new Dictionary<string, int>();
    //[SerializeField] private Dictionary<string, int> AuctionPrice = new Dictionary<string, int>();
    //[SerializeField] private Dictionary<string, int> SeedPrice = new Dictionary<string, int>();
    //[SerializeField] private Dictionary<string, int> CropWarehouse = new Dictionary<string, int>();

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
            string saveData = JsonUtility.ToJson(data, true);
            filePath = Application.persistentDataPath + "/userdata.json";
            File.WriteAllText(filePath, saveData);

            data.CropsLevel.Add("lettuce", 1);
            data.CropsLevel.Add("spinach", 1);
            data.CropsLevel.Add("garlic", 1);

            data.Inventory.Add("promoter", 0);
            data.Inventory.Add("nutrients", 0);
            data.Inventory.Add("Rertilizer", 0);
            data.Inventory.Add("lettuce", 0);
            data.Inventory.Add("spinach", 0);
            data.Inventory.Add("garlic", 0);

            data.AuctionPrice.Add("lettuce", 0);
            data.AuctionPrice.Add("spinach", 0);
            data.AuctionPrice.Add("garlic", 0);

            data.SeedPrice.Add("lettuce", 10);

            data.CropWarehouse.Add("lettuce", 0);
            data.CropWarehouse.Add("spinach", 0);
            data.CropWarehouse.Add("garlic", 0);

            data.Microbe.Add("microbe1", 1);
            data.Microbe.Add("microbe2", 0);
            data.Microbe.Add("microbe3", 0);

            data.storage_count = 50;
            data.fieldCount = 9;
        }

        GameMoney = data.money;
        ui.ui.Name = data.name;
        ui.ui.Money = data.money;

        if(data.currentday != dt.Day)
        {
            AuctionPricing();
            Debug.Log(data.AuctionPrice["lettuce"]);
        }

        InitInventory();
        InitStorage();
        InstanceField();

        cultivate.Add("lettuce", false);
        cultivate.Add("microbe1", false);
        cultivate.Add("fertilizer", false);
    }

    private void SaveData()
    {
        data.money = GameMoney;
        data.currentday = dt.Day;

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
                        Debug.Log("fdafd");
                    }

                    else if(ExistFieldData != null && ExistFieldData.type == "" && ExistFieldData.microbe == "")
                    {
                        fm.Init(fieldNum);
                    }

                    else if(ExistFieldData != null && ExistFieldData.type != "" && ExistFieldData.microbe != "")
                    {
                        TimeSpan time = DateTime.Now - ExistFieldData.start_time;
                        fm.Init(fieldNum, ExistFieldData.type, ExistFieldData.microbe ,time);
                        Debug.Log(time);
                    }

                    else if (ExistFieldData != null && ExistFieldData.type != "" && ExistFieldData.microbe == "")
                    {
                        TimeSpan time = DateTime.Now - ExistFieldData.start_time;
                        fm.Init(fieldNum, ExistFieldData.type, ExistFieldData.microbe, time);
                        Debug.Log(time);
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
        data.AuctionPrice["lettuce"] = UnityEngine.Random.Range(30, 150);
        data.AuctionPrice["spinach"] = UnityEngine.Random.Range(200, 900);
        data.AuctionPrice["garlic"] = UnityEngine.Random.Range(500, 2500);
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
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == Laboratory)
                {
                    ui.Laboratory_Open();
                }

                else if(hit.collider.gameObject == Fertilizer_Facility)
                {

                }

                else if(hit.collider.gameObject == Storage)
                {
                    ui.StorageInstance();
                }
            }
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

    public void HarvestCrops(string name)
    {
        data.CropWarehouse[name] += 1;
        ui.AddStorage(name, 1, false);

        int random = UnityEngine.Random.Range(0, 1);
        if(random == 0)
        {
            data.Microbe["lettuce"] += 1;
            Debug.Log(data.Microbe["lettuce"]);
        }
    }

    public void Selling(string name, int num)
    {
        data.CropWarehouse[name] -= num;
        GameMoney += num * data.AuctionPrice[name];
        ui.ui.Money = GameMoney;

        ui.UpdateStorage(name, num);
    }

    public void Buying(string name, int num)
    {
        AddInventory(name);
        GameMoney -= num * data.SeedPrice[name];
        ui.ui.Money = GameMoney;
    }

    public TimeSpan Growth_Time_Manager(string name, int num)
    {
        FieldData ExistData = data.fieldDatas.Find(ExistData => ExistData.field_num == num);
        if (ExistData != null && ExistData.type == "")
        {
            ExistData.type = name;

            ExistData.start_time = DateTime.Now;
            ExistData.count = 0;
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
            ExistData.count = 0;
            ExistData.start_time = DateTime.MinValue;
        }
    }

    public void CultivationManager(string name)
    {
        switch (name)
        {
            case "lettuce":
                if (!cultivate["lettuce"])
                    cultivate["lettuce"] = true;
                else
                    cultivate["lettuce"] = false;
                break;

            case "microbe1":
                if (!cultivate["microbe1"])
                {
                    cultivate["microbe1"] = true;
                }
                else
                    cultivate["microbe1"] = false;
                break;

            case "fertilizer":

                break;
        }
    }

    private void MicrobeManager()
    {

    }
}
