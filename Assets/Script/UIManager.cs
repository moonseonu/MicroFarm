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

    public UserInfo ui = new UserInfo();

    [SerializeField] private TMP_Text Money_Text;
    [SerializeField] private TMP_Text UserName_Text;

    [SerializeField] private GameObject Bag_Close;
    [SerializeField] private GameObject Bag_Open;
    [SerializeField] private GameObject Shop;
    [SerializeField] private GameObject Empty_Slot;

    /// <summary>
    /// 0~2 : æ∆¿Ã≈€
    /// 3~5 : æææ—
    /// </summary>
    public bool[] isUsed = new bool[6];
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < isUsed.Length; i++)
        {
            isUsed[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        PrintInfo();
        //AddToggleGroup();
    }

    public void ButtonEvent(string name)
    {
        switch(name)
        {
            case "bag close":
                Bag_Close.SetActive(false);
                Bag_Open.SetActive(true);
                InventoryManager();
                break;

            case "bag open":
                Bag_Close.SetActive(true);
                Bag_Open.SetActive(false);
                break;

            case "shop":
                break;

            case "seed1":
                isUsed[3] = true;
                GameManager.instance.UseItem = true;
                GameManager.instance.InstanceInventoryItem();
                break;

            case "seed2":
                break;
        }
    }

    private void PrintInfo()
    {
        UserName_Text.text = ui.Name;
        Money_Text.text = ui.Money.ToString();
    }

    private void IsValueZero()
    {
        Dictionary<string, int> inven = GameManager.instance.GetInventory();
        
    }
    private void InventoryManager()
    {
        Dictionary<string, int> inven = GameManager.instance.GetInventory();
        GameObject[] item = new GameObject[6];
        int i = 0;
        foreach (string key in inven.Keys)
        {
            if (inven[key] != 0)
            {
                if (key == "lettuce seed")
                {
                    item[i] = GameManager.instance.InstanceInven("lettuce seed");
                    GameObject temp = Instantiate(item[i]);
                    temp.transform.parent = Bag_Open.transform;
                    i++;
                }
            }

            else
            {
                item[i] = Empty_Slot;
                GameObject temp = Instantiate(item[i]);
                temp.transform.parent = Bag_Open.transform;
                i++;
            }
        }
    }
}
