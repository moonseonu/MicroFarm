using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    private int num;
    public int Num
    {
        get { return num; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (name == "Lettuce Storage")
        {
            num = GameManager.instance.CropsNum("lettuce");
            Debug.Log(num);
        }
    }
}
