using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public string Cult_Name;
    // Start is called before the first frame update
    void Start()
    {

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
                                Debug.Log("Seed_Lt is cultivated");
                                break;
                        }
                    }
                }
            }
        }
    }
}
