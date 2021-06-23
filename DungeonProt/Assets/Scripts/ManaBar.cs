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

    Player player;

    void Start()
    {
        player = GetComponent<Player>();

        fill = 1f;
        currentMp = player.ManaPoints;
        maxMp = player.MaxMana;
    }
    
    void Update()
    {
        currentMp = player.ManaPoints;

        if (currentMp < 0) currentMp = 0;
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
