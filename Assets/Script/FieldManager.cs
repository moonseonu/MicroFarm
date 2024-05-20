using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private float time;
    private Dictionary<string, bool> Growth_Type = new Dictionary<string, bool>();
    private Dictionary<string, bool> Used_Microbe = new Dictionary<string, bool>();
    private int FieldNum;

    private Dictionary<string, float> Grow_Time = new Dictionary<string, float>();

    [SerializeField] private GameObject Lettuce;

    [SerializeField] private GameObject GrowingLettuce0;
    [SerializeField] private GameObject GrowingLettuce1;
    [SerializeField] private GameObject GrowingLettuce2;
    [SerializeField] private GameObject GrowingLettuce3;

    private bool isHarvest;
    public bool isMenu;
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
        Used_Microbe.Add("microbe1", false);

        Growth_Type[type] = true;

        Grow_Time.Add("lettuce", 10.0f);
        if (time >= TimeSpan.FromSeconds(Grow_Time[type] * 1 / 3))
        {
            Lettuce.SetActive(true);
            GrowingLettuce1.SetActive(true);
        }

        if (time >= TimeSpan.FromSeconds(Grow_Time[type] * 2 / 3))
        {
            GrowingLettuce1.SetActive(false);
            GrowingLettuce2.SetActive(true);
        }

        if (time >= TimeSpan.FromSeconds(Grow_Time[type]))
        {
            GrowingLettuce2.SetActive(false);
            GrowingLettuce3.SetActive(true);
            isHarvest = true;
        }
    }

    private void Cultivation()
    {
        UIManager ui = GameManager.instance.GetComponent<UIManager>();
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
                            if (!ui.isMenuOpen)
                            {
                                isMenu = ui.Open_Cultivation_Menu();
                            }
                        }

                        else
                        {
                            isMenu = ui.Open_Cultivation_Menu();

                            foreach (var item in GameManager.instance.cultivate)
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
                        GrowingLettuce0.SetActive(false);
                        GrowingLettuce1.SetActive(false);
                        GrowingLettuce2.SetActive(false);
                        GrowingLettuce3.SetActive(false);
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
        if (Growth_Type.ContainsKey("lettuce") && Growth_Type["lettuce"])
        {
            GrowingLettuce0.SetActive(true);
            if (!Used_Microbe["microbe1"] && Used_Microbe.ContainsKey("microbe1"))
            {
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 3))
                {
                    GrowingLettuce1.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 2 / 3))
                {
                    GrowingLettuce1.SetActive(false);
                    GrowingLettuce2.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"]))
                {
                    GrowingLettuce2.SetActive(false);
                    GrowingLettuce3.SetActive(true);
                    isHarvest = true;
                }
            }

            else
            {
                Debug.Log("2");
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 3 * 1 / 2))
                {
                    GrowingLettuce1.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 2 / 3 * 1 / 2))
                {
                    GrowingLettuce2.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 2))
                {
                    GrowingLettuce3.SetActive(true);
                    isHarvest = true;
                }
            }
        }
    }
}
