using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public class UserInfo
    {
        private string name;
        private int money;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Money
        {
            get { return money; }
            set { money = value; }
        }
    }

    public class Slot
    {
        public GameObject item;
        public int quantity;
        public TMP_Text quantity_text;
    }

    private List<Slot> slots = new List<Slot>();

    public UserInfo ui = new UserInfo();

    [SerializeField] private TMP_Text Money_Text;
    [SerializeField] private TMP_Text UserName_Text;

    [SerializeField] private GameObject Storage_Inven;
    [SerializeField] private GameObject Bag_Close;
    [SerializeField] private GameObject Bag_Open;
    [SerializeField] private GameObject Shop;

    [SerializeField] private GameObject Empty_Slot;
    [SerializeField] private GameObject Lettuce_Slot;

    [SerializeField] private List<GameObject> inventory_List;
    [SerializeField] private List<GameObject> Instanced_Inven;
    [SerializeField] private Dictionary<GameObject, int> StorageNum = new Dictionary<GameObject, int>();

    public void AddInvenList(GameObject item)
    {
        if(!inventory_List.Contains(item))
            inventory_List.Insert(0, item);
    }
    public void isFullList()
    {
        if(inventory_List.Count > 6) 
        {
            inventory_List.Remove(inventory_List[5]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UI_Init();
    }

    // Update is called once per frame
    void Update()
    {
        PrintInfo();
    }

    private void UI_Init()
    {
        for(int i = 0; i < 50; i++)
        {
            GameObject temp = Instantiate(Empty_Slot);
            temp.transform.parent = Storage_Inven.transform;
        }
    }

    public void ButtonEvent(string name)
    {
        switch(name)
        {
            case "bag close":
                Bag_Close.SetActive(false);
                Bag_Open.SetActive(true);
                InventoryInstance();
                break;

            case "bag open":
                Bag_Close.SetActive(true);
                Bag_Open.SetActive(false);
                break;

            case "shop":
                break;

            case "close storage":
                Storage_Inven.transform.parent.parent.parent.gameObject.SetActive(false);
                break;
        }
    }

    private void PrintInfo()
    {
        UserName_Text.text = ui.Name;
        Money_Text.text = ui.Money.ToString();
    }

    private void InventoryInstance()
    {
        foreach (GameObject item in inventory_List)
        {
            //한번 인스턴스 될 때 6개의 아이템만 인스턴스 되기 때문에 6개 이상이 되면 인스턴스 할 필요가 없음
            if (Instanced_Inven.Count < 6)
            {
                GameObject temp = Instantiate(item);
                temp.transform.parent = GameObject.Find("Bag Open").transform;
                Instanced_Inven.Add(item);
                Toggle togle = temp.GetComponent<Toggle>();
                if(togle != null)
                {
                    togle.onValueChanged.AddListener((value) => IsUsedItem(value, item));
                }
            }
        }
    }

    public void StorageInstance()
    {
        Storage_Inven.transform.parent.parent.parent.gameObject.SetActive(true);
        //for(int i = 0; i < StorageNum.Count; i++)
        //{
        //    GameObject temp = Storage_Inven.transform.GetChild(i).GetChild(0).gameObject;
        //    StorageManager sm = temp.GetComponent<StorageManager>();
        //    TMP_Text text = temp.transform.Find("Count").gameObject.GetComponent<TMP_Text>();
        //    text.text = StorageNum[Lettuce_Slot].ToString();
        //}

    }

    public void IsUsedItem(bool isUsed, GameObject item)
    {
        if (isUsed)
        {
            GameManager.instance.UseItem = true;
            GameManager.instance.IsUsedItem(item);
        }

        else
        {
            GameManager.instance.UseItem= false;
        }
    }

    public void AddStorage(string name, int num)
    {
        int count = num;
        GameObject temp = null;
        switch (name)
        {
            case "lettuce":

                Slot Existslot = slots.Find(Existslot => Existslot.item == Lettuce_Slot && Existslot.quantity < 2);
                if(Existslot != null)
                {
                    Existslot.quantity++;
                    Existslot.quantity_text.text = Existslot.quantity.ToString();
                    Debug.Log(Existslot.quantity);
                }
                else
                {
                    Slot Newslot = new Slot { item = Lettuce_Slot, quantity = 1 };
                    slots.Add(Newslot);
                    for (int i = 0; i < GameManager.instance.Storage_Count; i++)
                    {
                        if (Storage_Inven.transform.GetChild(i).childCount == 0)
                        {
                            
                            temp = Instantiate(Lettuce_Slot);
                            temp.transform.parent = Storage_Inven.transform.GetChild(i).transform;
                            break;
                        }
                    }
                    Newslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                    Newslot.quantity_text.text = Newslot.quantity.ToString();
                }
                break;
        }
    }
}
