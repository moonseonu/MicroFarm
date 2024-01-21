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
    [SerializeField] private List<GameObject> inventory_List;
    [SerializeField] private List<GameObject> Instanced_Inven;
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

    }

    // Update is called once per frame
    void Update()
    {
        PrintInfo();
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
            //�ѹ� �ν��Ͻ� �� �� 6���� �����۸� �ν��Ͻ� �Ǳ� ������ 6�� �̻��� �Ǹ� �ν��Ͻ� �� �ʿ䰡 ����
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
}
