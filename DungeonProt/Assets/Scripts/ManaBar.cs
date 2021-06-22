using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Image bar;
    public float fill;
    public Text output;
    public float maxMp;
    public float currentMp;
    void Start()
    {
        fill = 1f;
        currentMp = 100;
        maxMp = 100;
    }
    
    void Update()
    {
        if(currentMp < 0) currentMp = 0;
        if(currentMp > maxMp) currentMp = maxMp;
        fill = currentMp / maxMp;
        bar.fillAmount = fill;
        output.text = currentMp + "/" + maxMp;
    }
    
    public void EditManaValue(float amount)
    {
        currentMp += amount;
    }
    
}
