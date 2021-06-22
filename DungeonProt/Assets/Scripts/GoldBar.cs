using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldBar : MonoBehaviour
{
    public Text text;
    public float money;
    void Start()
    {
    //    money = 100;
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = money + "$"; 
    }
}
