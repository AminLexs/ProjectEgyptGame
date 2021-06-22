using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    public float fill;
    public Text output;
    public float maxHp;
    public float currentHp;
    void Start()
    {
        fill = 1f;
        currentHp = 100;
        maxHp = 100;
    }
    
    void Update()
    {
        if(currentHp < 0) currentHp = 0;
        if(currentHp > maxHp) currentHp = maxHp;
        fill = currentHp / maxHp;
        bar.fillAmount = fill;
        output.text = currentHp + "/" + maxHp;
    }
    
    public void EditHpValue(float amount)
    {
        currentHp += amount;
    }
    
}
