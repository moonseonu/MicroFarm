using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{

    private SpriteRenderer Sr;
    [SerializeField] private float time;
    private Dictionary<string, bool> Growth_Type = new Dictionary<string, bool>();
    private Dictionary<string, bool> Used_Microbe = new Dictionary<string, bool>();
    private int FieldNum;

    private Dictionary<string, float> Grow_Time = new Dictionary<string, float>();

    [SerializeField] private GameObject Lettuce;

    [SerializeField] private Sprite GrowingLettuce1;
    [SerializeField] private Sprite GrowingLettuce2;
    [SerializeField] private Sprite GrowingLettuce3;

    [SerializeField] private GameObject GrowingLettuce11;
    [SerializeField] private GameObject GrowingLettuce22;
    [SerializeField] private GameObject GrowingLettuce33;

    private bool isHarvest;
    private bool isMenu;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Cultivation();
        Growth();
        //Harvest();
    }

    public void Init(int num)
    {
        FieldNum = num;
        isHarvest = false;
        isMenu = false;
        Growth_Type.Add("lettuce", false);
        Growth_Type.Add("spinach", false);
        Growth_Type.Add("garlic", false);

        Used_Microbe.Add("microbe1", false);

        Grow_Time.Add("lettuce", 30.0f);
    }

    public void Init(int num, string type, string microbe ,TimeSpan time)
    {
        FieldNum = num;
        isHarvest = false;
        Growth_Type.Add("lettuce", false);
        Growth_Type.Add("spinach", false);
        Growth_Type.Add("garlic", false);

        Grow_Time.Add("lettuce", 10.0f);
        if (time >= TimeSpan.FromSeconds(Grow_Time[type] * 1 / 3))
        {
            Lettuce.SetActive(true);
            GrowingLettuce11.SetActive(true);
        }

        if (time >= TimeSpan.FromSeconds(Grow_Time[type] * 2 / 3))
        {
            GrowingLettuce22.SetActive(true);
        }

        if (time >= TimeSpan.FromSeconds(Grow_Time[type]))
        {
            GrowingLettuce33.SetActive(true);
            isHarvest = true;
        }
    }
    private void Cultivation()
    {
        if (!isHarvest)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        if (!isMenu)
                        {
                            UIManager ui = GameManager.instance.GetComponent<UIManager>();
                            ui.Open_Cultivation_Menu();
                            isMenu = true;
                        }

                        else
                        {
                            UIManager ui = GameManager.instance.GetComponent<UIManager>();
                            ui.Open_Cultivation_Menu();
                            isMenu = false;

                            foreach(var item in GameManager.instance.cultivate)
                            {
                                if(item.Value == true)
                                {
                                    if(item.Key == "lettuce")
                                    {
                                        if (GameManager.instance.Inventory_Count("lettuce") != 0)
                                        {
                                            Lettuce.SetActive(true);
                                            if (!isHarvest)
                                            {
                                                if (!Growth_Type["lettuce"])
                                                {
                                                    GameManager.instance.UseInventory("lettuce");
                                                }
                                            }
                                            Growth_Type[item.Key] = true;
                                        }
                                    }

                                    else if(item.Key == "microbe1")
                                    {
                                        if(GameManager.instance.Microbe_Count(item.Key) != 0)
                                        {
                                            Used_Microbe[item.Key] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {

                        GrowingLettuce11.SetActive(false);
                        GrowingLettuce22.SetActive(false);
                        GrowingLettuce33.SetActive(false);
                        Lettuce.SetActive(false);
                        GameManager.instance.HarvestCrops("lettuce");
                        isHarvest = false;
                        Growth_Type["lettuce"] = false;
                        time = 0.0f;
                        GameManager.instance.FieldDataInit(FieldNum);
                    }
                }
            }
        }
    }

    private void Growth()
    {
        if (Growth_Type["lettuce"])
        {
            if (!Used_Microbe["microbe1"])
            {
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 3))
                {
                    GrowingLettuce11.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 2 / 3))
                {
                    GrowingLettuce22.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"]))
                {
                    GrowingLettuce33.SetActive(true);
                    isHarvest = true;
                }
            }

            else
            {
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 3 * 1 / 2))
                {
                    GrowingLettuce11.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 2 / 3 * 1 / 2))
                {
                    GrowingLettuce22.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 2))
                {
                    GrowingLettuce33.SetActive(true);
                    isHarvest = true;
                }
            }
        }
    }
}
