using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultureManager : MonoBehaviour
{
    public string type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Culturing();
    }

    private void Culturing()
    {
        if(type == "Sample1")
        {
            GameManager.instance.UpdateCulture(type);
        }
    }
}
