using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
        public float fresh_time;
    }

    private class Inventory
    {
        public GameObject item;
        public int quantity;
        public TMP_Text quantity_text;
        public Toggle toggle;
    }

    [SerializeField] private List<Slot> slots = new List<Slot>();
    [SerializeField] private List<GameObject> UIslot = new List<GameObject>();
    [SerializeField] private List<Inventory> inventorys = new List<Inventory>();
    [SerializeField] private GameObject Cultivation_Menu_Object;

    public UserInfo ui = new UserInfo();

    [SerializeField] private TMP_Text Money_Text;
    [SerializeField] private TMP_Text UserName_Text;

    [SerializeField] private GameObject Storage_Inven;
    [SerializeField] private GameObject Laboratory_Board;
    [SerializeField] private GameObject Bag_Content;
    [SerializeField] private GameObject Bag_Open;
    [SerializeField] private GameObject Shop;
    [SerializeField] private GameObject Notice_Board;

    [SerializeField] private GameObject Empty_Slot;
    [SerializeField] private GameObject Lettuce_Slot;
    [SerializeField] private GameObject Rotten_Slot;

    [SerializeField] private Dictionary<string, GameObject> StorageSlot = new Dictionary<string, GameObject>();

    [SerializeField] private List<GameObject> inventory_List;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PrintInfo();
        IsFreshSlot();
    }

    public void UI_Init()
    {
        GameObject temp = null;
        //창고 인벤토리
        for(int i = 0; i < 50; i++)
        {
            temp = Instantiate(Empty_Slot);
            temp.transform.parent = Storage_Inven.transform;
        }

        //가방 인벤토리
        for(int i = 0; i < 6; i++)
        {
            temp = Instantiate(Empty_Slot);
            temp.transform.parent = Bag_Content.transform;
        }

        StorageSlot.Add("lettuce", Lettuce_Slot);
        StorageSlot.Add("rotten", Rotten_Slot);
    }

    public void Open_Cultivation_Menu()
    {
        if (!Cultivation_Menu_Object.activeSelf)
        {
            Cultivation_Menu_Object.SetActive(true);
        }

        else
        {
            Cultivation_Menu_Object.SetActive(false);
        }
    }

    public void ButtonEvent(string name)
    {
        switch (name)
        {
            case "bag close":
                Bag_Content.SetActive(false);
                break;

            case "bag":
                Bag_Content.SetActive(true);
                break;

            case "shop":
                Shop.SetActive(true);
                break;

            case "close storage":
                Storage_Inven.transform.parent.parent.parent.gameObject.SetActive(false);
                break;

            case "close shop":
                Shop.SetActive(false);
                break;

            case "buy lettuce":
                GameManager.instance.Buying("lettuce", 1);
                break;

            case "notice board open":
                Notice_Board.SetActive(true);
                break;

            case "notice board close":
                Notice_Board.SetActive(false);
                break;

            case "sell lettuce":
                SellCrop("lettuce", 1);
                break;

            case "close lab":
                Laboratory_Board.SetActive(false);
                break;
        }
    }

    private void PrintInfo()
    {
        UserName_Text.text = ui.Name;
        Money_Text.text = ui.Money.ToString();
    }

    public void StorageInstance()
    {
        Storage_Inven.transform.parent.parent.parent.gameObject.SetActive(true);
    }

    public void Laboratory_Open()
    {
        Laboratory_Board.SetActive(true);
    }

    public void IsUsedItem(bool isUsed, GameObject item)
    {
        if (isUsed)
        {
            GameManager.instance.UseItem = true;
            GameManager.instance.IsUsedItem(item.name);
            //UseItem_Inventory(item);
        }

        else
        {
            GameManager.instance.UseItem= false;
            GameManager.instance.IsUsedItem("");
            //UseItem_Inventory(null);
        }
    }

    private void MakeSlot(string name, int num)
    {
        GameObject temp = null;
        if(num != 1)
        {
            for (int i = 0; i < GameManager.instance.Storage_Count; i++)
            {
                if (Storage_Inven.transform.GetChild(i).childCount == 0)
                {

                    temp = Instantiate(StorageSlot[name]);
                    temp.transform.parent = Storage_Inven.transform.GetChild(i).transform;
                    break;
                }
            }
            Slot Newslot = new Slot { item = StorageSlot[name], quantity = 2 };
            slots.Add(Newslot);
            Newslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
            Newslot.quantity_text.text = Newslot.quantity.ToString();
            num -= 2;
        }

        else
        {
            for (int i = 0; i < GameManager.instance.Storage_Count; i++)
            {
                if (Storage_Inven.transform.GetChild(i).childCount == 0)
                {

                    temp = Instantiate(StorageSlot[name]);
                    temp.transform.parent = Storage_Inven.transform.GetChild(i).transform;
                    break;
                }
            }
            Slot Newslot = new Slot { item = StorageSlot[name], quantity = 1 };
            slots.Add(Newslot);
            Newslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
            Newslot.quantity_text.text = Newslot.quantity.ToString();
            num = 0;
        }
        if (num > 2)
            MakeSlot(name, num);
        else if(num == 1)
            MakeSlot(name, 1);
    }

    public void AddStorage(string name, int num, bool isInit)
    {
        GameObject temp = null;
        switch (name)
        {
            case "lettuce":
                if (!isInit)
                {
                    Slot Existslot = slots.Find(Existslot => Existslot.item == StorageSlot[name] && Existslot.quantity < 2);
                    if (Existslot != null)
                    {
                        Existslot.quantity += num;
                        Existslot.quantity_text.text = Existslot.quantity.ToString();
                    }

                    else
                    {
                        for (int i = 0; i < GameManager.instance.Storage_Count; i++)
                        {
                            if (Storage_Inven.transform.GetChild(i).childCount == 0)
                            {

                                temp = Instantiate(StorageSlot[name]);
                                temp.transform.parent = Storage_Inven.transform.GetChild(i).transform;
                                break;
                            }
                        }
                        Slot Newslot = new Slot { item = StorageSlot[name], quantity = 1 };
                        slots.Add(Newslot);
                        Newslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                        Newslot.quantity_text.text = Newslot.quantity.ToString();
                    }

                }

                else
                {
                    if (num <= 2)
                    {
                        if(num != 0)
                        {
                            for (int i = 0; i < GameManager.instance.Storage_Count; i++)
                            {
                                if (Storage_Inven.transform.GetChild(i).childCount == 0)
                                {
                                    temp = Instantiate(StorageSlot[name]);
                                    temp.transform.parent = Storage_Inven.transform.GetChild(i).transform;
                                    break;
                                }
                            }
                            Slot Newslot = new Slot { item = StorageSlot[name], quantity = num };
                            slots.Add(Newslot);
                            Newslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                            Newslot.quantity_text.text = Newslot.quantity.ToString();
                        }
                      
                    }

                    else
                    {
                        MakeSlot(name, num);
                    }

                }
                break;
        }
    }

    public void AddInventory(GameObject item, int num, bool isInit)
    {
        GameObject temp = null;

        Inventory Existinven = inventorys.Find(Existinven => item);
        if (Existinven != null)
        {
            Existinven.quantity++;
            Existinven.quantity_text.text = Existinven.quantity.ToString();
        }
        else
        {
            if(!isInit)
            {
                Inventory Newinven = new Inventory { item = item, quantity = 1 };
                inventorys.Add(Newinven);
                if (!inventory_List.Contains(item))
                    inventory_List.Add(item);
                for (int i = 0; i < 6; i++)
                {
                    if (Bag_Content.transform.GetChild(i).childCount == 0)
                    {

                        temp = Instantiate(item);
                        temp.transform.parent = Bag_Content.transform.GetChild(i).transform;
                        break;
                    }
                }
                Newinven.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                Newinven.quantity_text.text = Newinven.quantity.ToString();

                Newinven.toggle = temp.GetComponent<Toggle>();
                if (Newinven.toggle != null)
                {
                    Newinven.toggle.onValueChanged.AddListener((value) => IsUsedItem(value, Newinven.item));
                }
            }
            else
            {
                if(num != 0)
                {
                    Inventory Newinven = new Inventory { item = item, quantity = num };
                    inventorys.Add(Newinven);
                    if (!inventory_List.Contains(item))
                        inventory_List.Add(item);
                    for (int i = 0; i < 6; i++)
                    {
                        if (Bag_Content.transform.GetChild(i).childCount == 0)
                        {

                            temp = Instantiate(item);
                            temp.transform.parent = Bag_Content.transform.GetChild(i).transform;
                            break;
                        }
                    }
                    Newinven.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                    Newinven.quantity_text.text = Newinven.quantity.ToString();

                    Newinven.toggle = temp.GetComponent<Toggle>();
                    if (Newinven.toggle != null)
                    {
                        Newinven.toggle.onValueChanged.AddListener((value) => IsUsedItem(value, Newinven.item));
                    }
                }
                
            }
        }
    }

    private void IsFreshSlot()
    {
        for(int i = 0; i <  slots.Count; i++)
        {
            if (slots[i].item.name == Lettuce_Slot.name)
            {
                slots[i].fresh_time += Time.deltaTime;
                if (slots[i].fresh_time > 100000.0f)
                {
                    slots[i].item = Rotten_Slot;
                    Transform slot = Storage_Inven.transform.GetChild(i).GetChild(0);
                    Destroy(slot.gameObject);
                    GameObject temp = Instantiate(Rotten_Slot);
                    temp.transform.parent = Storage_Inven.transform.GetChild(i).transform;
                    slots[i].quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                    slots[i].quantity_text.text = slots[i].quantity.ToString();
                }
            }
        }
    }

    public void UseItem(string name, int num)
    {
        for(int i = 0; i < inventorys.Count; i++)
        {
            for(int j = 0; j < inventory_List.Count; j++)
            {
                if (inventorys[i].item.name == inventory_List[j].name)
                {
                    if (inventorys[i].quantity <= num)
                    {
                        Transform removeitem = Bag_Content.transform.GetChild(i+1).GetChild(0);
                        Destroy(removeitem.gameObject);
                        inventorys.RemoveAt(i);
                    }
                    else
                    {
                        inventorys[i].quantity -= num;
                        inventorys[i].quantity_text.text = inventorys[i].quantity.ToString();
                        if (inventorys[i].quantity == 0)
                        {
                            Transform removeitem = Bag_Content.transform.GetChild(i+1).GetChild(0);
                            Destroy(removeitem.gameObject);
                            inventorys.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

    private void SellCrop(string name, int num)
    {
        GameManager.instance.Selling(name, num);

    }

    /// <summary>
    /// 아직은 1개씩만 판매하기 때문에 두 슬롯 모두 최대보다 많이 파는 경우는 하지 않음
    /// </summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    public void UpdateStorage(string name, int num)
    {
        GameObject temp = null;
        Slot Existslot = slots.Find(Existslot => Existslot.item == StorageSlot[name] && Existslot.quantity != 2);
        if (Existslot != null)
        {
            Debug.Log("fadfd");
            temp = Existslot.quantity_text.gameObject.transform.parent.gameObject;
            Existslot.quantity -= num;
            if (Existslot.quantity == 0)
            {
                slots.Remove(Existslot);
                Destroy(temp.gameObject);
            }
            else
            {
                Existslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                Existslot.quantity_text.text = Existslot.quantity.ToString();
            }
        }

        else
        {
            Existslot = slots.FindLast(Existslot => Existslot.item == StorageSlot[name] && Existslot.quantity == 2);
            if (Existslot != null)
            {
                temp = Existslot.quantity_text.gameObject.transform.parent.gameObject;
                Existslot.quantity -= num;
                Existslot.quantity_text = temp.transform.Find("Count").GetComponent<TMP_Text>();
                Existslot.quantity_text.text = Existslot.quantity.ToString();
            }
        }
    }
}
