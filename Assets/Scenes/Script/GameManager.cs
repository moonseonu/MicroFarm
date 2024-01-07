using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

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

    [SerializeField] private GameObject Field;

    [SerializeField] private int GameMoney;
    [SerializeField] private int[] Auction = new int[3];
    [SerializeField] private Dictionary<string, int>CropsLevel = new Dictionary<string, int>();
    [SerializeField] private Dictionary<string, int>Inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        CropsLevel.Add("lettuce", 1);
        CropsLevel.Add("spinach", 1);
        CropsLevel.Add("garlic", 1);

        Inventory.Add("promoter", 0);
        Inventory.Add("nutrients", 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AuctionPricing()
    {

    }

    private void TakeMoney(int money)
    {

    }

    private void CropsLevelUp(string name)
    {
        CropsLevel[name] += 1;
    }

    private void AddInventory(string name)
    {
        Inventory[name] += 1;
    }
}
