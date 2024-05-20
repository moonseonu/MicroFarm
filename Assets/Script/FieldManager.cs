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

    public bool isHarvest;
    public bool isGrowing;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Growth();
    }

    public void Init(int num)
    {
        FieldNum = num;
        isHarvest = false;
        isGrowing = false;
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
        isGrowing = true;
        Growth_Type.Add("lettuce", false);
        Growth_Type.Add("spinach", false);
        Growth_Type.Add("garlic", false);
        Used_Microbe.Add("microbe1", false);

        Growth_Type[type] = true;
        Used_Microbe[microbe] = true;

        Grow_Time.Add(type, 40.0f);
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
            isGrowing = false;
        }
    }

    public void cultivation(string name)
    {
        switch (name)
        {
            case "lettuce":
                Lettuce.SetActive(true);
                GameManager.instance.UseInventory("lettuce");
                Growth_Type[name] = true;
                isGrowing = true;
                break;

            case "microbe1":
                GameManager.instance.UseMicrobe(name, FieldNum);
                Used_Microbe[name] = true;
                break;
        }
    }

    public void harvesting(string name)
    {
        switch (name)
        {
            case "lettuce":
                GrowingLettuce0.SetActive(false);
                GrowingLettuce1.SetActive(false);
                GrowingLettuce2.SetActive(false);
                GrowingLettuce3.SetActive(false);
                Lettuce.SetActive(false);
                GameManager.instance.HarvestCrops("lettuce");
                isHarvest = false;
                Growth_Type["lettuce"] = false;
                Used_Microbe["microbe1"] = false;
                time = 0.0f;
                GameManager.instance.FieldDataInit(FieldNum);
                break;
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
                Grow_Time["lettuce"] -= 10;
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 1 / 3))
                {
                    GrowingLettuce1.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"] * 2 / 3))
                {
                    GrowingLettuce2.SetActive(true);
                }
                if (GameManager.instance.Growth_Time_Manager("lettuce", FieldNum) >= TimeSpan.FromSeconds(Grow_Time["lettuce"]))
                {
                    GrowingLettuce3.SetActive(true);
                    isHarvest = true;
                }
            }
        }
    }
}
