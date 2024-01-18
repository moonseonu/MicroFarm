using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public string State;
    UIManager ui;
    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("GameManager").GetComponent<UIManager>();
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
                        for (int i = 0; i < 6; i++)
                        {
                            if (ui.isUsed[i])
                            {
                                Debug.Log("½É¾ú´Ù");
                            }
                        }
                    }
                }
            }
        }
    }
}
