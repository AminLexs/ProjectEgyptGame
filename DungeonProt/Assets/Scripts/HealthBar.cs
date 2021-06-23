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

    Player player;

    void Start()
    {
        player = GetComponent<Player>();

        fill = 1f;
        currentHp = player.Health;
        maxHp = player.MaxHealth;
    }
    
    void Update()
    {
        currentHp = player.Health;

        if (currentHp < 0) currentHp = 0;
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
