using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private SpriteRenderer Sr;
    [SerializeField] private float time;
    private float lettuce_time1 = 0;
    private float lettuce_time2 = 10.0f;
    private float lettuce_time3 = 20.0f;
    [SerializeField] private Sprite GrowingLettuce1;
    [SerializeField] private Sprite GrowingLettuce2;
    [SerializeField] private Sprite GrowingLettuce3;
    // Start is called before the first frame update
    void Start()
    {
        Sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Cultivation();
    }

    public void Cultivation()
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
                                time += Time.deltaTime;
                                if(time > lettuce_time1)
                                {
                                    Sr.sprite = GrowingLettuce1;
                                }
                                if(time > lettuce_time2)
                                {
                                    Sr.sprite = GrowingLettuce2;
                                }
                                Debug.Log("Seed_Lt is cultivated");
                                break;
                        }
                    }
                }
            }
        }
    }
}
