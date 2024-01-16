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
    // Start is called before the first frame update
    void Start()
    {
        PrintInfo();
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
            case "bag":
                break;

            case "shop":
                break;

            case "seed1":
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
}
