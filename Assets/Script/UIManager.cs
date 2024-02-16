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
        public float fresh_time;
    }

    public class Inventory
    {
        public GameObject item;
        public int quantity;
        public TMP_Text quantity_text;
        public Toggle toggle;
    }

    [SerializeField] private List<Slot> slots = new List<Slot>();
    [SerializeField] private List<Inventory> inventorys = new List<Inventory>();

    public UserInfo ui = new UserInfo();

    [SerializeField] private TMP_Text Money_Text;
    [SerializeField] private TMP_Text UserName_Text;

    [SerializeField] private GameObject Storage_Inven;
    [SerializeField] private GameObject Bag_Content;
    [SerializeField] private GameObject Bag_Open;
    [SerializeField] private GameObject Shop;
    [SerializeField] private GameObject Notice_Board;

    [SerializeField] private GameObject Empty_Slot;
    [SerializeField] private GameObject Lettuce_Slot;
    [SerializeField] private GameObject Rotten_Slot;

    [SerializeField] private Dictionary<string, GameObject> StorageSlot = new Dictionary<string, GameObject>();

    [SerializeField] private List<GameObject> inventory_List;
    [SerializeField] private List<GameObject> Instanced_Inven;

    public void AddInvenList(GameObject item, int num)
    {
        if (!inventory_List.Contains(item))
        {
            inventory_List.Insert(0, item);
            TMP_Text item_Count = item.transform.Find("Count").GetComponent<TMP_Text>();
            item_Count.text = num.ToString();
        }
    }

    public void RemoveInvenList(GameObject item)
    {
        if (inventory_List.Contains(item))
        {
            int index = inventory_List.IndexOf(item);
            inventory_List[index] = Empty_Slot;
            GameObject temp = Instanced_Inven[index];
            Destroy(temp);
            Instanced_Inven.Remove(item);
        }
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
        IsFreshSlot();
        //InventoryInstance();
    }

    private void UI_Init()
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
        GameObject temp = null;
        switch (name)
        {
            case "lettuce":

                Slot Existslot = slots.Find(Existslot => Existslot.item == StorageSlot[name] && Existslot.quantity < 2);
                if(Existslot != null)
                {
                    Existslot.quantity++;
                    Existslot.quantity_text.text = Existslot.quantity.ToString();
                }
                else
                {
                    Slot Newslot = new Slot { item = StorageSlot[name], quantity = 1 };
                    slots.Add(Newslot);
                    for (int i = 0; i < GameManager.instance.Storage_Count; i++)
                    {
                        if (Storage_Inven.transform.GetChild(i).childCount == 0)
                        {
                            
                            temp = Instantiate(StorageSlot[name]);
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

    public void AddInventory(GameObject item, int num)
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
            Inventory Newinven = new Inventory { item = item, quantity = 1 };
            inventorys.Add(Newinven);
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
            Newinven.toggle = Newinven.item.GetComponent<Toggle>();
            if (Newinven.toggle != null)
            {
                Newinven.toggle.onValueChanged.AddListener((value) => IsUsedItem(value, Newinven.item));
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

    private void SellCrop(string name, int num)
    {
        switch (name)
        {
            case "lettuce":
                Debug.Log(slots.Count);
                for(int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].item.name == StorageSlot[name].name)
                    {
                        if (slots[i].quantity < num)
                        {
                            num -= slots[i].quantity;
                            slots[i].item = Rotten_Slot;
                            Transform slot = Storage_Inven.transform.GetChild(i).GetChild(0);
                            Destroy(slot.gameObject);
                            slots.RemoveAt(i);
                            Debug.Log(slots.Count);
                        }

                        else
                        {
                            slots[i].quantity -= num;
                            slots[i].quantity_text.text = slots[i].quantity.ToString();
                            if (slots[i].quantity == 0)
                            {
                                Transform slot = Storage_Inven.transform.GetChild(i).GetChild(0);
                                Destroy(slot.gameObject);
                                slots.RemoveAt(i);
                            }
                        }
                    }
                }
                GameManager.instance.Selling(name, num);
                break;
        }
    }
}
