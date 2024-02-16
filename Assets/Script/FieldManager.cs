using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private SpriteRenderer Sr;
    [SerializeField] private float time;
    private Dictionary<string, bool> Growth_Type = new Dictionary<string, bool>();

    private float lettuce_time1 = 0;
    private float lettuce_time2 = 1.0f;
    private float lettuce_time3 = 2.0f;

    [SerializeField] private GameObject Lettuce;

    [SerializeField] private Sprite GrowingLettuce1;
    [SerializeField] private Sprite GrowingLettuce2;
    [SerializeField] private Sprite GrowingLettuce3;

    private bool isHarvest;
    // Start is called before the first frame update
    void Start()
    {
        isHarvest = false;
        Growth_Type.Add("lettuce", false);
        Growth_Type.Add("spinach", false);
        Growth_Type.Add("garlic", false);
    }

    // Update is called once per frame
    void Update()
    {
        Cultivation();
        Growth();
        Harvest();
    }

    private void Cultivation()
    {
        if (GameManager.instance.UseItem)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        switch (GameManager.instance.usedName)
                        {
                            case "Seed_Lt":
                                Debug.Log("fdadf");
                                if (GameManager.instance.Inventory_Count("lettuce") != 0)
                                {
                                    Lettuce.SetActive(true);
                                    Sr = Lettuce.GetComponent<SpriteRenderer>();
                                    if (!isHarvest && !Growth_Type["lettuce"])
                                        GameManager.instance.UseInventory("lettuce");
                                    Growth_Type["lettuce"] = true;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    private void Growth()
    {
        if (Growth_Type["lettuce"])
        {
            time += Time.deltaTime;
            if (time > lettuce_time1)
            {
                Sr.sprite = GrowingLettuce1;
            }
            if (time > lettuce_time2)
            {
                Sr.sprite = GrowingLettuce2;
            }
            if (time > lettuce_time3)
            {
                Sr.sprite = GrowingLettuce3;
                isHarvest = true;
            }
        }
    }

    private void Harvest()
    {
        if (isHarvest)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        if (Growth_Type["lettuce"])
                        {
                            Sr.sprite = GrowingLettuce1;
                            Lettuce.SetActive(false);
                            GameManager.instance.HarvestCrops("lettuce");
                            isHarvest = false;
                            Growth_Type["lettuce"] = false;
                            time = 0.0f;
                        }
                    }
                }
            }
        }
    }
}
